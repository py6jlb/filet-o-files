using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.RecipeTag;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;

namespace FiletOFiles.Api.Features.RecipeTags.UpsertRecipeTags;

public static class UpdateRecipeTagEndpoint
{
    public static IEndpointRouteBuilder MapUpdateRecipeTag(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapPut("/{recipeId}/tags", HandleAsync)
            .WithName(nameof(UpdateRecipeTagEndpoint))
            .WithDescription("Обновить метки рецепта")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute] string recipeId,
        [FromBody] RecipeTagDto request,
        [FromServices] AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var recipe = await db
            .Recipes.Include(r => r.RecipeTags)
            .FirstOrDefaultAsync(r => r.Id == recipeId, cancellationToken: cancellationToken);

        if (recipe is null)
        {
            return TypedResults.Problem(
                detail: "Не найден рецепт",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        var currentTag = recipe.RecipeTags.FirstOrDefault(x => x.TagId == request.TagId);
        if (currentTag is null)
        {
            recipe.RecipeTags.Add(
                new RecipeTag
                {
                    RecipeId = recipeId,
                    TagId = request.TagId,
                    AdditionalData = request.AdditionalData,
                }
            );

            await db.SaveChangesAsync(cancellationToken);
        }
        else
        {
            currentTag.AdditionalData = request.AdditionalData;
            await db.SaveChangesAsync(cancellationToken);
        }
        return TypedResults.NoContent();
    }
}
