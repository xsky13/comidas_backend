using comidas_backend.Models.Dto.Request;
using comidas_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace comidas_backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController(IUserService userService): ControllerBase
{
    [HttpPost("/login")]
    public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await userService.LoginUser(request.Email, request.Contrasena);
        if (!result.Success) return BadRequest(result.Error);
        
        Response.Cookies.Append("X-Access-Token", result.Value!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        
        return Ok();
    }
}