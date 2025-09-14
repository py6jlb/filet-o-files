using System;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Helpers;
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
        endpointRouteBuilder
            .MapPost("/login", Handle)
            .WithName(nameof(LoginEndpoint))
            .WithDescription("Вход")
            .Produces<AccessTokenDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> Handle(
        [FromServices] AuthService service,
        [FromBody] LoginUserDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.Login(request, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : ErrorHelper.GetProblem(result.Errors[0]);
    }
}
