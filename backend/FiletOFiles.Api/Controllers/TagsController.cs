using FiletOFiles.Api.DTOs.Common;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Features.Tags.AddTag;
using FiletOFiles.Api.Features.Tags.DeleteTag;
using FiletOFiles.Api.Features.Tags.GetTag;
using FiletOFiles.Api.Features.Tags.GetTags;
using FiletOFiles.Api.Features.Tags.UpdateTag;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FiletOFiles.Api.Controllers;

[Authorize]
[Route("tags")]
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
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Errors[0]?.Message);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<TagDto>>> GetTags(
        [FromServices] IGetTagsHandler handler,
        TagsQueryParameters request,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.GetTags(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Errors[0]?.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromServices] IAddTagHandler handler,
        [FromServices] IValidator<CreateTagDto> validator,
        CreateTagDto request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var result = await handler.AddTag(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTag), new { id = result.Value.Id }, result.Value)
            : Problem(result.Errors[0]?.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(
        [FromServices] IUpdateTagHandler handler,
        [FromServices] IValidator<UpdateTagDto> validator,
        string id,
        UpdateTagDto request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var result = await handler.Update(id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors[0]?.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromServices] IDeleteTagHandler handler,
        string id,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Delete(id, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors[0]?.Message);
    }
}
