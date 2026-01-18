using System;
using System.Security.Cryptography;
using System.Text;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.DTOs.AuthManagement;
using FiletOFiles.Api.DTOs.Users;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Settings;
using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace FiletOFiles.Api.Services;

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

        if (!identityUser.IsApproved)
        {
            return Result.Fail(
                new Error(
                    "Учетная запись не подтверждена. Обратитесь к администратору"
                ).WithMetadata("code", 401)
            );
        }

        if (identityUser.MustChangePassword)
        {
            return Result.Fail(
                new Error("Необходимо сменить пароль пользователя").WithMetadata("code", 409)
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

    public async Task<Result<string>> AddIdentityUser(
        RegisterUserDto request,
        bool asAdmin = false,
        bool isApproved = false,
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

        var identityUser = new AppIdentityUser
        {
            Email = request.Email,
            UserName = request.Name,
            IsApproved = isApproved,
            MustChangePassword = !isApproved,
        };
        var identityResult = await _userManager.CreateAsync(identityUser, request.Password);
        if (!identityResult.Succeeded)
        {
            return Result.Fail<string>(
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
            return Result.Fail<string>(
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
                return Result.Fail<string>(
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
        await _identityDb.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result.Ok(user.Id);
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

    public async Task<Result> ChangePassword(string userId, ChangePasswordDto request)
    {
        var identityUser = await _userManager.FindByIdAsync(userId);

        if (identityUser is null)
        {
            return Result.Fail(new Error("Пользователь не найдёт.").WithMetadata("code", 404));
        }

        var passwordCheckResult = await _userManager.CheckPasswordAsync(
            identityUser,
            request.OldPassword
        );

        if (!passwordCheckResult)
        {
            return Result.Fail(new Error("Неверный пароль").WithMetadata("code", 400));
        }

        var identityResult = await _userManager.ChangePasswordAsync(
            identityUser,
            request.OldPassword,
            request.NewPassword
        );

        if (!identityResult.Succeeded)
        {
            return Result.Fail(
                new Error("Не удалось сменить пароль.")
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
        ;

        return Result.Ok();
    }
}
