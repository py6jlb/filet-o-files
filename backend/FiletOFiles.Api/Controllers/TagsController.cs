using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Features.AddTag;
using FiletOFiles.Api.Features.GetTags;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly ILogger<TagsController> _logger;

    public TagsController(ILogger<TagsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] IGetTagsHandler handler,
        string textFragment
    )
    {
        var result = await handler.GetTags(textFragment);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromServices] IAddTagHandler handler,
        TagDto request
    )
    {
        var result = await handler.AddTag(request);
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
