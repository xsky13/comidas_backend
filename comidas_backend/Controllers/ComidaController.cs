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
    
    [HttpGet("proposals")]
    public async Task<ActionResult<List<Comida>>> GetUserProposals()
    {
        var userId = User.GetUserId();
        return Ok(await comidaService.GetProposals(userId));
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
    
    [HttpPost("fromProposal")]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50mb limit
    public async Task<ActionResult<Comida>> CreateFromProposal([FromForm] CreateComidaRequestDto request)
    {
        // crear comida de propuesta
        var userId = User.GetUserId();
        var response = await comidaService.CreateComida(request, false, userId);
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
}