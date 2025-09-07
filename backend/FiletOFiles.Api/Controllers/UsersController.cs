using FiletOFiles.Api.DTOs.Users;
using FiletOFiles.Api.Features.GetUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(
        string id,
        [FromServices] IGetUserHandler handler
    )
    {
        var result = await handler.GetUser(id);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
    }
}
