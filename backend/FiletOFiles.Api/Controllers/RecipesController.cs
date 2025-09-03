using System.Threading.Tasks;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Features.AddRecipe;
using FiletOFiles.Api.Features.GetRecipe;
using FiletOFiles.Api.Features.GetRecipes;
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
        string id
    )
    {
        var result = await handler.GetRecipe(id);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpGet]
    public async Task<ActionResult<RecipesCollectionDto>> GetRecipes(
        [FromServices] IGetRecipesHandler handler,
        RecipeQueryParameters request
    )
    {
        var result = await handler.GetRecipes(request);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddRecipe(
        [FromServices] IAddRecipeHandler handler,
        RecipeDto request
    )
    {
        var result = await handler.AddRecipe(request);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpPut]
    public IActionResult Update()
    {
        return Ok("Пока не реализовано");
    }

    [HttpDelete]
    public IActionResult Delete()
    {
        return Ok("Пока не реализовано");
    }
}
