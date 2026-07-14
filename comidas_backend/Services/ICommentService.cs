using comidas_backend.Models.Dto.Entity;
using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface ICommentService
{
    public Task<List<ComentarioDto>> GetCommentsByFood(int comidaId);
    public Task<Result<ComentarioDto>> CreateComment(int comidaId, int userId, string textoComentario);
}