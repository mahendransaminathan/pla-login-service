
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

using System.Linq;

[Authorize(AuthenticationSchemes = "AzureAD,AdminCookie")]  // Requires Azure AD SSO login
[Route("admin")]
[ApiController]
public class AdminController : ControllerBase
{
    // GET /admin
    [HttpGet("")]
    public IActionResult Index()
    {
        // Return a simple message confirming admin access
        return Ok("Welcome to the Admin area - SSO authenticated");
    }

    // GET /admin/profile
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        // Return some basic info about the logged-in user from claims
        var userName = User.Identity?.Name ?? "Unknown";
        var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(new
        {
            Message = "Admin Profile Info",
            UserName = userName,
            Claims = userClaims
        });
    }

    // POST /admin/logout
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Sign out from the cookie and the OpenID Connect provider
        return SignOut("AdminCookie", "AzureAD");
    }
}
