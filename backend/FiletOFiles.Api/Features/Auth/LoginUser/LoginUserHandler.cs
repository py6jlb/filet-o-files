using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using FiletOFiles.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FiletOFiles.Api.Features.LoginUser;

public class LoginUserHandler : ILoginUserHandler
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<LoginUserHandler> _logger;
    private readonly AppDbContext _db;
    private readonly AppDbIdentityContext _identityDb;
    private readonly TokenProvider _tokenProvider;
    private readonly JwtAuthOptions _jwtAuthOptions;

    public LoginUserHandler(
        ILogger<LoginUserHandler> logger,
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
            return Result.Failure<AccessTokenDto>("Ошибка входа пользователя");
        }

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!);
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

        return Result.Success(accessTokens);
    }
}
