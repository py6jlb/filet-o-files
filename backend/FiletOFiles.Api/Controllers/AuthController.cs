using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Features.LoginUser;
using FiletOFiles.Api.Features.RegisterUser;
using FiletOFiles.Api.Features.TokenRefresh;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace FiletOFiles.Api.Controllers;

[Route("auth")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AccessTokenDto>> Register(
        [FromServices] IRegisterUserHandler handler,
        RegisterUserDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Register(request, cancellationToken);
        return result.IsSuccess ? Ok(result) : Problem(result.Error);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenDto>> Login(
        [FromServices] ILoginUserHandler handler,
        LoginUserDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Login(request, cancellationToken);
        return result.IsSuccess ? Ok(result) : Unauthorized(result.Error);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AccessTokenDto>> Refresh(
        [FromServices] IRefreshTokenHandler handler,
        RefreshTokenDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Refresh(request, cancellationToken);
        return result.IsSuccess ? Ok(result) : Unauthorized(result.Error);
    }
}
