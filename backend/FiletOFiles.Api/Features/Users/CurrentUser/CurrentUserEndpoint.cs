using System;
using System.Security.Claims;
using FiletOFiles.Api.DTOs.Users;
using FiletOFiles.Api.Extensions;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Users.CurrentUser;

public static class CurrentUserEndpoint
{
    public static IEndpointRouteBuilder MapCurrentUserUser(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGet("/me", HandleAsync)
            .WithName(nameof(CurrentUserEndpoint))
            .WithDescription("Получить текущего пользователя")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        HttpContext context,
        ClaimsPrincipal principal,
        [FromServices] AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        string userId = context.GetUserId();
        var user = await db
            .Users.AsNoTracking()
            .Where(x => x.IdentityId == userId)
            .Select(UserMapping.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }
}
