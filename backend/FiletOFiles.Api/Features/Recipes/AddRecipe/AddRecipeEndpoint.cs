using System;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Recipes.AddRecipe;

public static class AddRecipeEndpoint
{
    public static IEndpointRouteBuilder MapAddRecipe(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapPost("", HandleAsync)
            .WithName(nameof(MapAddRecipe))
            .Produces<RecipeDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        CreateRecipeDto request,
        [FromServices] AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var newRecipe = request.ToEntity();
        await db.Recipes.AddAsync(newRecipe, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        var result = newRecipe.ToDto();

        return TypedResults.Ok(result);
    }
}
