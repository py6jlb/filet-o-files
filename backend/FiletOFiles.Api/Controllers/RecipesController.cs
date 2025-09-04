using System.Threading.Tasks;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Features.AddRecipe;
using FiletOFiles.Api.Features.GetRecipe;
using FiletOFiles.Api.Features.GetRecipes;
using FiletOFiles.Api.Features.UpdateRecipe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class RecipesController : ControllerBase
{
    private readonly ILogger<RecipesController> _logger;

    public RecipesController(ILogger<RecipesController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDto>> GetRecipe(
        [FromServices] IGetRecipeHandler handler,
        string id,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.GetRecipe(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpGet]
    public async Task<ActionResult<RecipesCollectionDto>> GetRecipes(
        [FromServices] IGetRecipesHandler handler,
        RecipeQueryParameters request,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.GetRecipes(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddRecipe(
        [FromServices] IAddRecipeHandler handler,
        CreateRecipeDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.AddRecipe(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetRecipe), new { id = result.Value.Id }, result.Value)
            : Problem(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateRecipeDto request,
        [FromServices] IUpdateRecipeHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Update(id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Error);
    }

    [HttpDelete]
    public IActionResult Delete()
    {
        return Ok("Пока не реализовано");
    }
}
