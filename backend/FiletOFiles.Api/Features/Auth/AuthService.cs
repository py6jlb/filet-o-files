using System;
using System.Security.Claims;
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
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FiletOFiles.Api.Features.Auth;

public sealed class AuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AuthService> _logger;
    private readonly AppDbContext _db;
    private readonly AppDbIdentityContext _identityDb;

    public AuthService(
        ILogger<AuthService> logger,
        AppDbContext db,
        AppDbIdentityContext identityDb,
        UserManager<IdentityUser> userManager
    )
    {
        _userManager = userManager;
        _db = db;
        _logger = logger;
        _identityDb = identityDb;
    }

    public async Task<Result<ClaimsIdentity>> Register(
        RegisterUserDto request,
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

        var identityUser = new IdentityUser { Email = request.Email, UserName = request.Name };

        var identityResult = await _userManager.CreateAsync(identityUser, request.Password);
        if (!identityResult.Succeeded)
        {
            return Result.Fail<ClaimsIdentity>(
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

        var user = request.ToEntity();
        user.IdentityId = identityUser.Id;

        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role
        );

        identity
            .SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(identityUser))
            .SetClaim(Claims.Email, await _userManager.GetEmailAsync(identityUser))
            .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(identityUser))
            .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(identityUser))
            .SetClaims(Claims.Role, [.. await _userManager.GetRolesAsync(identityUser)]);

        identity.SetScopes(new[] { Scopes.OpenId, Scopes.Email, Scopes.Profile, Scopes.Roles });
        return Result.Ok(identity);
    }
}
