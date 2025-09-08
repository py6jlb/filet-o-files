using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using Microsoft.AspNetCore.Identity;

namespace FiletOFiles.Api.Features.LoginUser;

public class LoginUserHandler : ILoginUserHandler
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<LoginUserHandler> _logger;
    private readonly AppDbContext _db;
    private readonly AppDbIdentityContext _identityDb;
    private readonly TokenProvider _tokenProvider;

    public LoginUserHandler(
        ILogger<LoginUserHandler> logger,
        AppDbContext db,
        AppDbIdentityContext identityDb,
        UserManager<IdentityUser> userManager,
        TokenProvider tokenProvider
    )
    {
        _userManager = userManager;
        _db = db;
        _logger = logger;
        _identityDb = identityDb;
        _tokenProvider = tokenProvider;
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
        var accessToken = _tokenProvider.Create(tokenRequest);

        return Result.Success(accessToken);
    }
}
