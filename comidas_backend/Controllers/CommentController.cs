using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Services;
using comidas_backend.Utils;
using Microsoft.AspNetCore.Mvc;

namespace comidas_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController(ICommentService commentService) : ControllerBase
{
    [HttpGet("{comidaId}")]
    public async Task<ActionResult<List<ComentarioDto>>> GetComentarios(int comidaId)
    {
        var comentarios = await commentService.GetCommentsByFood(comidaId);
        return comentarios;
    }

    [HttpPost("{comidaId}")]
    public async Task<ActionResult<ComentarioDto>> CreateComentario(
        [FromBody] CreateCommentRequestDto request,
        int comidaId
    )
    {
        var userId = User.GetUserId();
        var result = await commentService.CreateComment(comidaId, userId, request.TextoComentario);
        return result.ToActionResult();
    }
    
    [HttpDelete("{comidaId}")]
    public async Task<ActionResult<object>> DeleteComentario(int comidaId)
    {
        var userId = User.GetUserId();
        var result = await commentService.DeleteComment(comidaId, userId);
        return result.ToActionResult();
    }
}