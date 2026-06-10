using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Services;
using comidas_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace comidas_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService): ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        var userId = User.GetUserId();
        var response = await userService.GetUserById(userId);
        
        return response.ToActionResult();
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await userService.LoginUser(request.Email, request.Contrasena);
        if (!result.Success) return BadRequest(new { Error = result.Error, Field = result.Field});
        
        Response.Cookies.Append("X-Access-Token", result.Value!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        
        return Ok();
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        Response.Cookies.Delete("X-Access-Token");
        return Ok();
    }
}