
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _userService;

    public LoginController(ILoginService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Login request)
    {
        var user = _userService.Authenticate(request);
        if (user == null)
            return Unauthorized("Invalid username or password.");

        var token = _userService.GenerateJwtToken(user);
        return Ok(new LoginResponse(token, user.Username, DateTime.UtcNow, "Customer", true));
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] Login request)
    {
        _userService.Register(request);
        return Ok("User registered successfully.");
    }
}
