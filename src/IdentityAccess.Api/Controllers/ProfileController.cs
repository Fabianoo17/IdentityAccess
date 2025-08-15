using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAccess.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        var name = User.Identity?.Name ?? "anonymous";
        var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        return Ok(new { user = name, roles });
    }
}