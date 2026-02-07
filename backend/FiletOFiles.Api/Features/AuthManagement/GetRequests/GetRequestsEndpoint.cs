using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.AuthManagement;
using FiletOFiles.Api.DTOs.Users;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.AuthManagement.GetRequests;

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
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromServices] UserManager<AppIdentityUser> userManager,
        [FromServices] AppDbContext db,
        CancellationToken cancellationToken = default
    )
    {
        var usersIdentityIds = await userManager
            .Users.Where(x => x.IsApproved == false)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        if (usersIdentityIds.Length == 0)
        {
            return TypedResults.Ok(Array.Empty<UserDto>());
        }

        var users = await db
            .Users.Where(u => usersIdentityIds.Contains(u.IdentityId))
            .Select(UserMapping.ProjectToDto())
            .ToArrayAsync(cancellationToken);

        return TypedResults.Ok(users);
    }
}
