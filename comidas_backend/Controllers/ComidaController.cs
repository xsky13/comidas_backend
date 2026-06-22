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
        var userId = User.GetUserId();
        return Ok(await comidaService.GetComidas(userId));
    }
    
    [HttpGet("byPromedio")]
    public async Task<ActionResult<List<Comida>>> GetAll([FromQuery] string order)
    {
        var userId = User.GetUserId();
        return order == "desc" ? 
            Ok(await comidaService.GetComidasDesc(userId)) : 
            Ok(await comidaService.GetComidasAsc(userId));
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")] // fix to dynamic
    [RequestSizeLimit(50 * 1024 * 1024)] // 50mb limit
    public async Task<ActionResult<Comida>> Create([FromForm] CreateComidaRequestDto request)
    {
        var userId = User.GetUserId();
        var response = await comidaService.CreateComida(request, true, userId);
        return response.ToActionResult();
    }    

    [HttpPost("{id}/rate")]
    [Authorize]
    public async Task<ActionResult<object>> RateFood([FromBody] RateComidaRequestDto request, int id)
    {
        var userId = User.GetUserId();
        var response = await comidaService.RateComida(request, id, userId);
        return response.ToActionResult();
    }
    
    [HttpDelete("{id}/unrate")]
    [Authorize]
    public async Task<ActionResult<object>> UnrateFood(int id)
    {
        var userId = User.GetUserId();
        var response = await comidaService.UnrateComida(id, userId);
        return response.ToActionResult();
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> DeleteFood(int id)
    {
        var userId = User.GetUserId();
        var response = await comidaService.DeleteComida(id, userId);
        return response.ToActionResult();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50mb limit
    public async Task<ActionResult<object>> UpdateFood([FromForm] UpdateComidaRequestDto request, int id)
    {
        var userId = User.GetUserId();
        var response = await comidaService.UpdateComida(request, id, userId);
        return response.ToActionResult();
    }
}