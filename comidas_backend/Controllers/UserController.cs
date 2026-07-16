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
    
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await userService.RegisterUser(request.Nombre, request.Email, request.Contrasena);
        if (!result.Success) return BadRequest(new { Error = result.Error, Field = result.Field});

        Response.Cookies.Append("X-Access-Token", result.Value!, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await userService.LoginUser(request.Email, request.Contrasena);
        if (!result.Success) return BadRequest(new { Error = result.Error, Field = result.Field});
        
        Response.Cookies.Append("X-Access-Token", result.Value!, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)

        });
        
#if DEBUG
       return Ok(new { Token = result.Value });
#else
        return Ok();
#endif
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<UserDto>> UpdateUser([FromBody] UpdateUserRequestDto request)
    {
        var userId = User.GetUserId();
        var result = await userService.UpdateUser(userId, request.Nombre, request.Email, request.Contrasena);
        return result.ToActionResult();
    }
    
    [Authorize]
    [HttpPut("changePassword")]
    public async Task<ActionResult<UserDto>> ChangePassword(ChangePasswordRequestDto request)
    {
        var userId = User.GetUserId();
        var result = await userService.ChangePassword(userId, request);
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        Response.Cookies.Delete("X-Access-Token");
        return Ok();
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult> DeleteUser()
    {
        var userId = User.GetUserId();
        var result = await userService.DeleteUser(userId);
        if (!result.Success) return BadRequest(new { Error = result.Error, Field = result.Field});
        Response.Cookies.Delete("X-Access-Token");
        
        return Ok(new { Message = result.Value });

    }
}