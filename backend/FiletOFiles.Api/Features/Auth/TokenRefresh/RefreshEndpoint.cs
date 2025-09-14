using System;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Auth.TokenRefresh;

public static class RefreshEndpoint
{
    public static IEndpointRouteBuilder MapRefresh(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("/refresh", Handle)
            .WithName(nameof(RefreshEndpoint))
            .WithDescription("Обновление токенов")
            .Produces<AccessTokenDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        ;
        return endpointRouteBuilder;
    }

    public static async Task<IResult> Handle(
        [FromServices] AuthService service,
        [FromBody] RefreshTokenDto request,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.Refresh(request, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : ErrorHelper.GetProblem(result.Errors[0]);
    }
}
