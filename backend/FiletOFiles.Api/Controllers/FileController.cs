using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;

    public FileController(ILogger<FileController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Пока не реализовано");
    }

    [HttpPost("search")]
    public IActionResult Search()
    {
        return Ok("Пока не реализовано");
    }

    [HttpPost]
    public IActionResult Post()
    {
        return Ok("Пока не реализовано");
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
