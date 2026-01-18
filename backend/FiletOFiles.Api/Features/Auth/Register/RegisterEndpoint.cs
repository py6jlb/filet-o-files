using System;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.DTOs.AuthManagement;
using FiletOFiles.Api.Helpers;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Auth.Register;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("/register", Handle)
            .WithName(nameof(RegisterEndpoint))
            .WithDescription("Регистрация")
            .AllowAnonymous()
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> Handle(
        [FromServices] AuthService service,
        [FromBody] RegisterUserDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.AddIdentityUser(request, false, false, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : ErrorHelper.GetProblem(result.Errors[0]);
    }
}
