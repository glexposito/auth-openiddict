using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Authorize]
public class ExistentialController : ControllerBase
{
    [HttpGet("~/whoami"), Produces("application/json")]
    public IActionResult WhoAmI()
    {
        return Ok($"You are {User.Identity?.Name}.");
    }
}