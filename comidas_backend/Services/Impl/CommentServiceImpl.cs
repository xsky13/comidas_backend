using comidas_backend.Data;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace comidas_backend.Services.Impl;

public class CommentServiceImpl(ComidasDbContext dbContext, ILogger<CommentServiceImpl> logger) : ICommentService
{
    public async Task<List<ComentarioViewDto>> GetCommentsByFood(int comidaId, int userId)
    {
        var comments = await dbContext.Comentarios
            .Where(c => c.ComidaId == comidaId)
            .Select(c => new ComentarioViewDto
            {
                Id = c.Id,
                ComidaId = c.ComidaId,
                Fecha = c.Fecha,
                Texto = c.Texto,
                UserId = c.UserId,
                Votos = c.Votos,
                UserVote = c.ListaVotos
                    .Where(v => v.UserId == userId)
                    .Select(v => (int?)v.VotoValue)
                    .FirstOrDefault() ?? 0,
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
            .OrderByDescending(c => c.Votos)
            .ToListAsync();
        return comments;
    }

    public async Task<Result<ComentarioViewDto>> CreateComment(int comidaId, int userId, string textoComentario)
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
        
        return Result<ComentarioViewDto>.Ok(new ComentarioViewDto
        {
            Id = newComentario.Id,
            ComidaId = newComentario.ComidaId,
            Fecha = newComentario.Fecha,
            Texto = newComentario.Texto,
            UserId = newComentario.UserId,
            Votos = newComentario.Votos,
            UserVote = 0,
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

    public async Task<Result<object>> VoteComment(int comentarioId, int userId, int voteValue)
    {
        // voteValue is 1 or -1
        var userVote = await dbContext.Votos
            .FirstOrDefaultAsync(v => v.ComentarioId == comentarioId && v.UserId == userId);

        int delta;
        if (userVote != null && userVote.VotoValue == voteValue)
        {
            delta = -voteValue; // undo the vote
        }
        else if (userVote != null)
        {
            delta = voteValue * 2; // flip from one side to the other
        }
        else
        {
            delta = voteValue; // new vote
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        var rows = await dbContext.Comentarios.Where(c => c.Id == comentarioId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Votos, c => c.Votos + delta));
        if (rows == 0)
        {
            await transaction.RollbackAsync();
            return Result<object>.Fail("Comentario no existe.");
        }

        if (userVote != null && userVote.VotoValue == voteValue)
        {
            await dbContext.Votos.Where(v => v.Id == userVote.Id).ExecuteDeleteAsync();
        }
        else if (userVote != null)
        {
            await dbContext.Votos.Where(v => v.Id == userVote.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(v => v.VotoValue, voteValue));
        }  else
        {
            try
            {
                dbContext.Votos.Add(new Voto { VotoValue = voteValue, UserId = userId, ComentarioId = comentarioId });
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                logger.LogWarning(ex, "Vote update failed for comentario {ComentarioId}, user {UserId}", comentarioId, userId);
                return Result<object>.Fail("Ocurrio un error.");
            }
        }

        await transaction.CommitAsync();
        return Result<object>.Ok(new { Success = true });
    }
    
}