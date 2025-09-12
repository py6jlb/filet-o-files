using System;
using CSharpFunctionalExtensions.HttpResults.ResultExtensions;
using FiletOFiles.Api.DTOs.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Auth.RegisterUser;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("register", Handle).WithName(nameof(RegisterEndpoint));
        return endpointRouteBuilder;
    }

    public static async Task<Results<Ok<AccessTokenDto>, ProblemHttpResult>> Handle(
        [FromServices] AuthService service,
        RegisterUserDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.Register(request, cancellationToken);
        return result.ToOkHttpResult(401);
    }
}
