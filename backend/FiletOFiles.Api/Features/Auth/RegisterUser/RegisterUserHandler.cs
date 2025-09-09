using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.DTOs.Users;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using FiletOFiles.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace FiletOFiles.Api.Features.RegisterUser;

public class RegisterUserHandler : IRegisterUserHandler
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly AppDbContext _db;
    private readonly AppDbIdentityContext _identityDb;
    private readonly TokenProvider _tokenProvider;
    private readonly JwtAuthOptions _jwtAuthOptions;

    public RegisterUserHandler(
        ILogger<RegisterUserHandler> logger,
        AppDbContext db,
        AppDbIdentityContext identityDb,
        UserManager<IdentityUser> userManager,
        TokenProvider tokenProvider,
        IOptions<JwtAuthOptions> jwtAuthOptions
    )
    {
        _userManager = userManager;
        _db = db;
        _logger = logger;
        _identityDb = identityDb;
        _tokenProvider = tokenProvider;
        _jwtAuthOptions = jwtAuthOptions.Value;
    }

    public async Task<Result<AccessTokenDto>> Register(
        RegisterUserDto request,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _identityDb.Database.BeginTransactionAsync(cancellationToken);
        _db.Database.SetDbConnection(_identityDb.Database.GetDbConnection());
        await _db.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        var identityUser = new IdentityUser { Email = request.Email, UserName = request.Name };

        var identityResult = await _userManager.CreateAsync(identityUser, request.Password);
        if (!identityResult.Succeeded)
        {
            return Result.Failure<AccessTokenDto>("Ошибка регистрации пользователя");
        }

        var user = request.ToEntity();
        user.IdentityId = identityUser.Id;

        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email);
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
        return Result.Success(accessTokens);
    }
}
