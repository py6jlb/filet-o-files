using System;
using CSharpFunctionalExtensions.HttpResults.ResultExtensions;
using FiletOFiles.Api.DTOs.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Auth.TokenRefresh;

public static class RefreshEndpoint
{
    public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("register", Handle).WithName(nameof(RefreshEndpoint));
        return endpointRouteBuilder;
    }

    public static async Task<Results<Ok<AccessTokenDto>, ProblemHttpResult>> Handle(
        [FromServices] AuthService service,
        RefreshTokenDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.Refresh(request, cancellationToken);
        return result.ToOkHttpResult(401);
    }
}
