using FiletOFiles.Api.DTOs.RecipeTag;
using FiletOFiles.Api.Features.RemoveRecipeTag;
using FiletOFiles.Api.Features.UpsertRecipeTags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Controllers;

[Authorize]
[Route("recipes/{recipeId}/tags")]
[ApiController]
public class RecipeTagsController : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult> UpsertRecipeTags(
        [FromServices] IUpsertRecipeTagsHandler handler,
        string recipeId,
        UpsertRecipeTagsDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Upsert(recipeId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Error);
    }

    [HttpDelete("{tagId}")]
    public async Task<ActionResult> RemoveRecipeTag(
        [FromServices] IRemoveRecipeTagHandler handler,
        string recipeId,
        string tagId,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Remove(recipeId, tagId, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Error);
    }
}
