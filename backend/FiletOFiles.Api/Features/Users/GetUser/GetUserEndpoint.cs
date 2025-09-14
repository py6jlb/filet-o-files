using System;
using FiletOFiles.Api.DTOs.Users;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Users.GetUser;

public static class GetUserEndpoint
{
    public static IEndpointRouteBuilder MapGetUser(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapGet("/{id}", HandleAsync)
            .WithName(nameof(GetUserEndpoint))
            .WithDescription("Получить пользователя по ID")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute] string id,
        [FromServices] AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var user = await db
            .Users.Where(u => u.Id == id)
            .Select(UserMapping.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }
}
