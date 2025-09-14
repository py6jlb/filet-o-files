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
        [FromBody] UpsertRecipeTagsDto request,
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

        var currentTagIds = recipe.RecipeTags.Select(x => x.TagId).ToHashSet();
        if (currentTagIds.SetEquals(request.TagIds))
        {
            return TypedResults.Ok();
        }

        List<string> existingTagIds = await db
            .Tags.Where(t => request.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        if (existingTagIds.Count != request.TagIds.Count)
        {
            return TypedResults.Problem(
                detail: "One or more tag IDs is invalid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        recipe.RecipeTags.RemoveAll(rt => !request.TagIds.Contains(rt.TagId));
        var tagIdsToAdd = request.TagIds.Except(currentTagIds).ToArray();
        recipe.RecipeTags.AddRange(
            tagIdsToAdd.Select(tagId => new RecipeTag { RecipeId = recipeId, TagId = tagId })
        );

        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
