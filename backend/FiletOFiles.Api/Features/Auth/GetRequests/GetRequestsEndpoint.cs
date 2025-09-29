using System;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Auth.GetRequests;

public static class GetRequestsEndpoint
{
    public static IEndpointRouteBuilder MapGetRequests(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGet("/requests", HandleAsync)
            .WithName(nameof(GetRequestsEndpoint))
            .WithDescription("Заявки на регистрацию")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromServices] AppDbIdentityContext identityDb,
        CancellationToken cancellationToken = default
    )
    {
        var query = await identityDb
            .RegistrationRequests.Select(r => r.ToRegistrationRequestDto())
            .ToArrayAsync(cancellationToken);

        return TypedResults.Ok(query);
    }
}
