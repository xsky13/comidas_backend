using comidas_backend.Models.Dto.Entity;
using comidas_backend.Utils;

namespace comidas_backend.Services.Impl;

public class CommentServiceImpl : ICommentService
{
    public async Task<List<ComentarioDto>> GetCommentsByFood(int comidaId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ComentarioDto>> CreateComment(int comidaId, int userId, string textoComentario)
    {
        throw new NotImplementedException();
    }
}