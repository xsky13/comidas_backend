using comidas_backend.Data;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace comidas_backend.Services.Impl;

public class PropuestaServiceImpl(ComidasDbContext dbContext) : IPropuestaService
{
    
    public async Task<List<ComidaDto>> GetProposals(int userId)
    {
        // doble query para mas performance: primero hacemos select de la comida, y despues solo de la calificacion perteneciente al usuario
        // despues las seteamos en el dto, asi no tenemos que hacer loop a traves de todas las calificaciones
        var comidas = await dbContext.Comidas
            .Where(comida => !comida.Confirmada && comida.UserId == userId)
            .Select(comida => new
            {
                Comida = comida,
                Calificacion = comida.Calificacions
                    .Where(c => c.UserId == userId)
                    .Select(c => (int?)c.Cantidad)
                    .SingleOrDefault()
                
            })
            .Select(result => new ComidaDto()
            {
                Id = result.Comida.Id,
                Titulo = result.Comida.Titulo,
                ImgUrl = result.Comida.ImgUrl,
                UserId = result.Comida.UserId,
                CantidadCalificaciones = result.Comida.CantidadCalificaciones,
                PromedioEstrellas = result.Comida.PromedioEstrellas,
                Confirmada = result.Comida.Confirmada,
                UsuarioCalifica = result.Calificacion != null,
                CalificacionUsuario = result.Calificacion,
                DateCreated = result.Comida.DateCreated
            })
            .OrderByDescending(comida => comida.DateCreated)
            .ToListAsync();
        
        return comidas;
    }
    
    public async Task<Result<Comida>> UpdateProposal(int userId, string userRole, int comidaId, string newTitle)
    {
        var comida = await dbContext.Comidas.FindAsync(comidaId);
        if (comida == null) return Result<Comida>.Fail("No existe la comida");
        
        // solo la puede editar un admin, y el usuario solo si le pertenece y no esta confirmada
        if (userRole != "Admin" || (!comida.Confirmada && comida.UserId != userId))
            return Result<Comida>.Fail("No tiene permisos para editar esta comida");

        comida.Titulo = newTitle;
        await dbContext.SaveChangesAsync();

        return Result<Comida>.Ok(comida);
    }
}