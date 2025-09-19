using System;
using System.Security.Claims;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace FiletOFiles.Api.Features.Auth.RegisterUser;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("/register", Handle)
            .WithName(nameof(RegisterEndpoint))
            .WithDescription("Регистрация")
            .Produces<AccessTokenDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        ;
        return endpointRouteBuilder;
    }

    public static async Task<IResult> Handle(
        [FromServices] AuthService service,
        [FromBody] RegisterUserDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.Register(request, cancellationToken);
        if (result.IsSuccess)
        {
            result.Value.SetDestinations(AuthHelpers.GetDestinations);

            return TypedResults.SignIn(
                new ClaimsPrincipal(result.Value),
                null,
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
            );
        }
        else
        {
            return ErrorHelper.GetProblem(result.Errors[0]);
        }
    }
}
