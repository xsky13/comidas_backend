using comidas_backend.Models.Dto.Entity;
using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface ICommentService
{
    public Task<List<ComentarioViewDto>> GetCommentsByFood(int comidaId, int userId);
    public Task<Result<ComentarioViewDto>> CreateComment(int comidaId, int userId, string textoComentario);
    public Task<Result<object>> DeleteComment(int comentarioId, int userId);
    public Task<Result<object>> UpvoteComment(int comentarioId, int userId);
    public Task<Result<object>> DownvoteComment(int comentarioId, int userId);
}