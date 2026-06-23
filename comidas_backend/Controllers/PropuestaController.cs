using System.Security.Claims;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Services;
using comidas_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace comidas_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropuestaController(IPropuestaService propuestaService, IComidaService comidaService) : ControllerBase
{
    
    // seccion propuestas
    
    [HttpGet]
    public async Task<ActionResult<List<PropuestaDto>>> GetUserProposals()
    {
        var userId = User.GetUserId();
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;
        return Ok(await propuestaService.GetProposals(userId, userRole));
    }
    
    [HttpPost]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50mb limit
    public async Task<ActionResult<Comida>> CreateFromProposal([FromForm] CreateComidaRequestDto request)
    {
        // crear comida de propuesta
        var userId = User.GetUserId();
        var response = await comidaService.CreateComida(request, false, userId);
        return response.ToActionResult();
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<Comida>> UpdateProposal([FromForm] UpdateProposalRequestDto request, int id)
    {
        var userId = User.GetUserId();
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;
        
        var response = await propuestaService.UpdateProposal(userId, userRole, id, request.Titulo);
        return response.ToActionResult();
    }

    [HttpPost("{id}/accept")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> AcceptProposal(int id)
    {
        var response = await propuestaService.AcceptProposal(id);
        return response.ToActionResult();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<Comida>> DeleteProposal(int id)
    {
        var userId = User.GetUserId();
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;
        
        var response = await propuestaService.DeleteProposal(userId, userRole, id);
        return response.ToActionResult();
    }
}