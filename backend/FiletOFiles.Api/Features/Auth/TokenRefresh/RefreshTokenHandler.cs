using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Features.TokenRefresh;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using FiletOFiles.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FiletOFiles.Api.Features.TokenRefresh;

public class RefreshTokenHandler : IRefreshTokenHandler
{
    private readonly ILogger<RefreshTokenHandler> _logger;
    private readonly AppDbContext _db;
    private readonly AppDbIdentityContext _identityDb;
    private readonly TokenProvider _tokenProvider;
    private readonly JwtAuthOptions _jwtAuthOptions;

    public RefreshTokenHandler(
        ILogger<RefreshTokenHandler> logger,
        AppDbContext db,
        AppDbIdentityContext identityDb,
        TokenProvider tokenProvider,
        IOptions<JwtAuthOptions> jwtAuthOptions
    )
    {
        _db = db;
        _logger = logger;
        _identityDb = identityDb;
        _tokenProvider = tokenProvider;
        _jwtAuthOptions = jwtAuthOptions.Value;
    }

    public async Task<Result<AccessTokenDto>> Refresh(
        RefreshTokenDto request,
        CancellationToken cancellationToken = default
    )
    {
        var refreshToken = await _identityDb
            .RefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null)
        {
            return Result.Failure<AccessTokenDto>("Пользователь не авторизован");
        }

        if (refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Result.Failure<AccessTokenDto>("Токен обновления просрочен");
        }

        var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!);
        var accessTokens = _tokenProvider.Create(tokenRequest);

        refreshToken.Token = accessTokens.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(
            _jwtAuthOptions.RefreshTokenExpirationDays
        );

        await _identityDb.SaveChangesAsync(cancellationToken);
        return Result.Success(accessTokens);
    }
}
