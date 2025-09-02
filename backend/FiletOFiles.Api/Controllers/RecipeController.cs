using System.Threading.Tasks;
using FiletOFiles.Api.Features.AddRecipe;
using FiletOFiles.Api.Features.GetRecipe;
using FiletOFiles.Api.Features.GetRecipes;
using FiletOFiles.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipeController : ControllerBase
{
    private readonly ILogger<RecipeController> _logger;

    public RecipeController(ILogger<RecipeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromServices] IGetRecipeHandler handler, long id)
    {
        var result = await handler.GetRecipe(id);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromServices] IGetRecipesHandler handler,
        GetRecipesRequest request
    )
    {
        var result = await handler.GetRecipes(request);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromServices] IAddRecipeHandler handler,
        AddRecipeRequest request
    )
    {
        var result = await handler.AddRecipe(request);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpPut]
    public IActionResult Put()
    {
        return Ok("Пока не реализовано");
    }

    [HttpDelete]
    public IActionResult Delete()
    {
        return Ok("Пока не реализовано");
    }
}
