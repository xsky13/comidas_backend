using comidas_backend.Data;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace comidas_backend.Services.Impl;

public class ComidaServiceImpl(ComidasDbContext dbContext, IFileService fileService) : IComidaService
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