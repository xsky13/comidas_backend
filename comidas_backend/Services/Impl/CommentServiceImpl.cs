using comidas_backend.Data;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace comidas_backend.Services.Impl;

public class CommentServiceImpl(ComidasDbContext dbContext) : ICommentService
{
    public async Task<List<ComentarioDto>> GetCommentsByFood(int comidaId)
    {
        var comments = await dbContext.Comentarios
            .Where(c => c.ComidaId == comidaId)
            .Select(c => new ComentarioDto
            {
                Id = c.Id,
                ComidaId = c.ComidaId,
                Fecha = c.Fecha,
                Texto = c.Texto,
                UserId = c.UserId,
                Votos = c.Votos,
                User = c.UserId == null ? new UserDto
                {
                    Id = 0,
                    Nombre = "Usuario no registrado",
                    Email = "Usuario no registrado",
                    Rol = UserRole.User
                }: new UserDto
                {
                    Id = c.User!.Id,
                    Nombre = c.User.Nombre,
                    Email = c.User.Email,
                    Rol = c.User.Rol
                }
            })
            .ToListAsync();
        return comments;
    }

    public async Task<Result<ComentarioDto>> CreateComment(int comidaId, int userId, string textoComentario)
    {
        var newComentario = new Comentario
        {
            ComidaId = comidaId,
            Fecha = DateTime.UtcNow,
            Texto = textoComentario,
            UserId = userId,
            Votos = 0
        };

        dbContext.Comentarios.Add(newComentario);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(newComentario).Reference(c => c.User).LoadAsync();
        
        return Result<ComentarioDto>.Ok(new ComentarioDto
        {
            Id = newComentario.Id,
            ComidaId = newComentario.ComidaId,
            Fecha = newComentario.Fecha,
            Texto = newComentario.Texto,
            UserId = newComentario.UserId,
            Votos = newComentario.Votos,
            User = new UserDto
            {
                Id = newComentario.User!.Id,
                Nombre = newComentario.User.Nombre,
                Email = newComentario.User.Email,
                Rol = newComentario.User.Rol
            }
        });
    }

    public async Task<Result<object>> DeleteComment(int comentarioId, int userId)
    {
        var comentario = await dbContext.Comentarios.FindAsync(comentarioId);
        
        if (comentario == null)
            return Result<object>.Fail("El comentario no existe.");

        if (comentario.UserId != userId)
            return Result<object>.Fail("No puede modificar este comentario");

        dbContext.Comentarios.Remove(comentario);
        await dbContext.SaveChangesAsync();

        return Result<object>.Ok(new { Success = true });
    }
}