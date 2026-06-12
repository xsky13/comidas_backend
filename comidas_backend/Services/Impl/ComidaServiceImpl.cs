using comidas_backend.Data;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comidas_backend.Services.Impl;

public class ComidaServiceImpl(ComidasDbContext dbContext, IFileService fileService) : IComidaService
{
    public async Task<List<ComidaDto>> GetComidas(int userId)
    {
        // doble query para mas performance: primero hacemos select de la comida, y despues solo de la calificacion perteneciente al usuario
        // despues las seteamos en el dto, asi no tenemos que hacer loop a traves de todas las calificaciones
        var comidas = await dbContext.Comidas
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
            })
            .ToListAsync();
        
        return comidas;
    }

    public async Task<Result<Comida>> CreateComida(CreateComidaRequestDto request, bool confirmada, int userId)
    {
        if (string.IsNullOrEmpty(request.Titulo))
            return Result<Comida>.Fail("El titulo no puede estar vacio", field: "titulo");
        
        // validar archivo
        var fileValidation = await fileService.VerifySingle(request.File);
        if (!fileValidation.Success) return Result<Comida>.Fail(fileValidation.Error);
        
        // crear archivo
        var returnedUrl = await fileService.CreateFile(request.File, userId);
        if (!returnedUrl.Success) return Result<Comida>.Fail(returnedUrl.Error);

        var comida = new Comida()
        {
            Titulo = request.Titulo,
            ImgUrl = returnedUrl.Value,
            PromedioEstrellas = 0,
            CantidadCalificaciones = 0,
            Confirmada = confirmada,
            UserId = userId
        };

        dbContext.Comidas.Add(comida);
        await dbContext.SaveChangesAsync();

        return Result<Comida>.Ok(comida);
    }
}