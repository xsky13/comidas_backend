using comidas_backend.Models.Dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace comidas_backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController: ControllerBase
{
    [HttpPost("/login")]
    public async Task<ActionResult> Login(LoginRequestDto request)
    {
        
        return Ok();
    }
}