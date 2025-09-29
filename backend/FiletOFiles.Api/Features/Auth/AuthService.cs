using System;
using System.Security.Cryptography;
using System.Text;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.DTOs.Users;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using FiletOFiles.Api.Settings;
using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace FiletOFiles.Api.Features.Auth;

public sealed class AuthService
{
    private readonly UserManager<AppIdentityUser> _userManager;
    private readonly ILogger<AuthService> _logger;
    private readonly AppDbContext _db;
    private readonly AppDbIdentityContext _identityDb;
    private readonly TokenProvider _tokenProvider;
    private readonly AuthOptions _jwtAuthOptions;

    public AuthService(
        ILogger<AuthService> logger,
        AppDbContext db,
        AppDbIdentityContext identityDb,
        UserManager<AppIdentityUser> userManager,
        TokenProvider tokenProvider,
        IOptions<AuthOptions> jwtAuthOptions
    )
    {
        _userManager = userManager;
        _db = db;
        _logger = logger;
        _identityDb = identityDb;
        _tokenProvider = tokenProvider;
        _jwtAuthOptions = jwtAuthOptions.Value;
    }

    public async Task<Result<AccessTokenDto>> Login(
        LoginUserDto request,
        CancellationToken cancellationToken = default
    )
    {
        var identityUser = await _userManager.FindByEmailAsync(request.Email);
        if (
            identityUser is null
            || !await _userManager.CheckPasswordAsync(identityUser, request.Password)
        )
        {
            return Result.Fail(new Error("Ошибка входа пользователя").WithMetadata("code", 401));
        }

        var roles = await _userManager.GetRolesAsync(identityUser);

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!, roles);
        var accessTokens = _tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = accessTokens.RefreshToken,
            UserId = identityUser.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
        };

        await _identityDb.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _identityDb.SaveChangesAsync(cancellationToken);

        return Result.Ok(accessTokens);
    }

    public async Task<Result<AccessTokenDto>> Register(
        RegisterUserDto request,
        bool asAdmin = false,
        CancellationToken cancellationToken = default
    )
    {
        bool emailIsTaken = await _userManager.FindByEmailAsync(request.Email) is not null;
        if (emailIsTaken)
        {
            return Result.Fail(
                new Error($"Email '{request.Email}' is already taken").WithMetadata("code", 409)
            );
        }

        bool usernameIsTaken = await _userManager.FindByNameAsync(request.Name) is not null;

        if (usernameIsTaken)
        {
            return Result.Fail(
                new Error($"Username '{request.Name}' is already taken").WithMetadata("code", 409)
            );
        }

        using var transaction = await _identityDb.Database.BeginTransactionAsync(cancellationToken);
        _db.Database.SetDbConnection(_identityDb.Database.GetDbConnection());
        await _db.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        var identityUser = new AppIdentityUser { Email = request.Email, UserName = request.Name };

        var identityResult = await _userManager.CreateAsync(identityUser, request.Password);
        if (!identityResult.Succeeded)
        {
            return Result.Fail<AccessTokenDto>(
                new Error("Ошибка регистрации пользователя")
                    .WithMetadata(
                        "extensions",
                        new Dictionary<string, object?>()
                        {
                            {
                                "errors",
                                identityResult.Errors.ToDictionary(
                                    x => x.Code,
                                    x => new[] { x.Description }
                                )
                            },
                        }
                    )
                    .WithMetadata("code", 400)
            );
        }

        var addToMemberRoleResult = await _userManager.AddToRoleAsync(identityUser, Roles.Member);
        if (!addToMemberRoleResult.Succeeded)
        {
            return Result.Fail<AccessTokenDto>(
                new Error("Невозможно зарегистрировать пользователя, попробуйте еще раз.")
                    .WithMetadata(
                        "extensions",
                        new Dictionary<string, object?>()
                        {
                            {
                                "errors",
                                addToMemberRoleResult.Errors.ToDictionary(
                                    x => x.Code,
                                    x => new[] { x.Description }
                                )
                            },
                        }
                    )
                    .WithMetadata("code", 400)
            );
        }

        if (asAdmin)
        {
            var addToAdminRoleResult = await _userManager.AddToRoleAsync(identityUser, Roles.Admin);
            if (!addToAdminRoleResult.Succeeded)
            {
                return Result.Fail<AccessTokenDto>(
                    new Error("Невозможно зарегистрировать пользователя, попробуйте еще раз.")
                        .WithMetadata(
                            "extensions",
                            new Dictionary<string, object?>()
                            {
                                {
                                    "errors",
                                    addToAdminRoleResult.Errors.ToDictionary(
                                        x => x.Code,
                                        x => new[] { x.Description }
                                    )
                                },
                            }
                        )
                        .WithMetadata("code", 400)
                );
            }
        }

        var user = request.ToEntity();
        user.IdentityId = identityUser.Id;

        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        IEnumerable<string> roles = asAdmin ? [Roles.Member, Roles.Admin] : [Roles.Member];
        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email, roles);
        var accessTokens = _tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = accessTokens.RefreshToken,
            UserId = identityUser.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
        };

        await _identityDb.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _identityDb.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return Result.Ok(accessTokens);
    }

    public async Task<Result<AccessTokenDto>> Refresh(
        RefreshTokenDto request,
        CancellationToken cancellationToken = default
    )
    {
        var refreshToken = await _identityDb
            .RefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null || refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Result.Fail<AccessTokenDto>(
                new Error("Пользователь не авторизован").WithMetadata("code", 401)
            );
        }
        var roles = await _userManager.GetRolesAsync(refreshToken.User);

        var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!, roles);
        var accessTokens = _tokenProvider.Create(tokenRequest);

        refreshToken.Token = accessTokens.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(
            _jwtAuthOptions.RefreshTokenExpirationDays
        );

        await _identityDb.SaveChangesAsync(cancellationToken);
        return Result.Ok(accessTokens);
    }

    public async Task<Result<AccessTokenDto>> HandleTelegramCallback(
        TelegramPayload payload,
        CancellationToken cancellationToken
    )
    {
        if (!ValidateTelegramAuth(payload))
        {
            return Result.Fail<AccessTokenDto>(
                new Error("Неверная подпись ответа телеграм").WithMetadata("code", 400)
            );
        }

        using var transaction = await _identityDb.Database.BeginTransactionAsync(cancellationToken);
        _db.Database.SetDbConnection(_identityDb.Database.GetDbConnection());
        await _db.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        var identityUser = await _userManager.FindByNameAsync(payload.username);

        if (identityUser == null)
        {
            var request = new RegistrationRequest
            {
                Id = $"rr_{Ulid.NewUlid()}",
                CreatedAtUtc = DateTime.UtcNow,
                TelegramId = payload.id,
                TelegramUserName = payload.username,
            };
            await _identityDb.RegistrationRequests.AddAsync(request, cancellationToken);

            identityUser = new AppIdentityUser
            {
                UserName = payload.username,
                TelegramId = payload.id,
                IsApproved = false,
            };
            var identityResult = await _userManager.CreateAsync(identityUser);
            if (!identityResult.Succeeded)
            {
                return Result.Fail<AccessTokenDto>(
                    new Error("Ошибка регистрации пользователя")
                        .WithMetadata(
                            "extensions",
                            new Dictionary<string, object?>()
                            {
                                {
                                    "errors",
                                    identityResult.Errors.ToDictionary(
                                        x => x.Code,
                                        x => new[] { x.Description }
                                    )
                                },
                            }
                        )
                        .WithMetadata("code", 400)
                );
            }

            var addToMemberRoleResult = await _userManager.AddToRoleAsync(
                identityUser,
                Roles.Member
            );
            if (!addToMemberRoleResult.Succeeded)
            {
                return Result.Fail<AccessTokenDto>(
                    new Error("Невозможно зарегистрировать пользователя, попробуйте еще раз.")
                        .WithMetadata(
                            "extensions",
                            new Dictionary<string, object?>()
                            {
                                {
                                    "errors",
                                    addToMemberRoleResult.Errors.ToDictionary(
                                        x => x.Code,
                                        x => new[] { x.Description }
                                    )
                                },
                            }
                        )
                        .WithMetadata("code", 400)
                );
            }

            var user = new User
            {
                Id = $"u_{Ulid.NewUlid()}",
                Name = payload.username,
                CreatedAtUtc = DateTime.UtcNow,
                IdentityId = identityUser.Id,
            };

            await _db.Users.AddAsync(user, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            await _identityDb.SaveChangesAsync(cancellationToken);
        }

        if (!identityUser.IsApproved)
        {
            await transaction.CommitAsync(cancellationToken);
            return Result
                .Ok()
                .WithReason(
                    new Success("Ваша заявка ожидает подтверждения администратора.").WithMetadata(
                        "code",
                        202
                    )
                );
        }

        var roles = await _userManager.GetRolesAsync(identityUser);

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!, roles);
        var accessTokens = _tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = accessTokens.RefreshToken,
            UserId = identityUser.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
        };

        await _identityDb.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _identityDb.SaveChangesAsync(cancellationToken);

        return Result.Ok(accessTokens);
    }

    private bool ValidateTelegramAuth(TelegramPayload payload)
    {
        var key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(_jwtAuthOptions.BotToken));
        var dataCheckString = string.Join(
            "\n",
            payload
                .GetType()
                .GetProperties()
                .Where(p => p.Name != "hash")
                .Select(p => $"{p.Name.ToLower()}={p.GetValue(payload)?.ToString()}")
                .OrderBy(s => s, StringComparer.Ordinal)
        );
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));

        var computedHash = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return computedHash == payload.hash.ToLower();
    }
}
