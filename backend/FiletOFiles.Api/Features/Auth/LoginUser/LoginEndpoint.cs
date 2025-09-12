using System;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.HttpResults.ResultExtensions;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using FiletOFiles.Api.Settings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FiletOFiles.Api.Features.Auth.LoginUser;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLogin(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("login", Handle).WithName(nameof(LoginEndpoint));
        return endpointRouteBuilder;
    }

    public static async Task<Results<Ok<AccessTokenDto>, ProblemHttpResult>> Handle(
        [FromServices] AuthService service,
        LoginUserDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.Login(request, cancellationToken);
        return result.ToOkHttpResult(401);
    }
}
