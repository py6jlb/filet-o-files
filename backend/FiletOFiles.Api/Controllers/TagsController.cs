using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Features.AddTag;
using FiletOFiles.Api.Features.GetTag;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(
        [FromServices] IGetTagHandler handler,
        string id,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.GetTag(id, cancellationToken);
        if (result.Value == null)
        {
            return NotFound();
        }
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpGet]
    public async Task<ActionResult<TagsCollectionDto>> GetTags(
        [FromServices] IGetTagsHandler handler,
        string textFragment,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.GetTags(textFragment, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromServices] IAddTagHandler handler,
        CreateTagDto request,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.AddTag(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTag), new { id = result.Value.Id }, result.Value)
            : Problem(result.Error);
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
