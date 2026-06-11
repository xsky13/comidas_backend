using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Services;
using comidas_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace comidas_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComidaController(IComidaService comidaService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Comida>>> GetAll()
    {
        return Ok(await comidaService.GetComidas());
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Comida>> Create([FromBody] CreateComidaRequestDto request)
    {
        var userId = User.GetUserId();
        var response = await comidaService.CreateComida(request, true, userId);
        return response.ToActionResult();
    }
}