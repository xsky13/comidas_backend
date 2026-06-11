using comidas_backend.Data;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace comidas_backend.Services.Impl;

public class ComidaServiceImpl(ComidasDbContext dbContext) : IComidaService
{
    public async Task<List<Comida>> GetComidas()
    {
        var comidas = await dbContext.Comidas.ToListAsync();
        return comidas;
    }

    public async Task<Result<Comida>> CreateComida(CreateComidaRequestDto request, bool confirmada, int userId)
    {
        if (string.IsNullOrEmpty(request.Titulo))
            return Result<Comida>.Fail("El titulo no puede estar vacio", field: "titulo");
        
        if (string.IsNullOrEmpty(request.ImgUrl))
            return Result<Comida>.Fail("La imagen no puede ser nula", field: "img_url");

        var comida = new Comida()
        {
            Titulo = request.Titulo,
            ImgUrl = request.ImgUrl,
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