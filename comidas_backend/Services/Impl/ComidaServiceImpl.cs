using comidas_backend.Data;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace comidas_backend.Services.Impl;

public class ComidaServiceImpl(ComidasDbContext dbContext, IFileService fileService) : IComidaService
{
    public async Task<List<ComidaDto>> GetComidas(int userId)
    {
        // doble query para mas performance: primero hacemos select de la comida, y despues solo de la calificacion perteneciente al usuario
        // despues las seteamos en el dto, asi no tenemos que hacer loop a traves de todas las calificaciones
        var comidas = await dbContext.Comidas
            .Where(comida => comida.Confirmada && comida.Activa)
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
                Descripcion = result.Comida.Descripcion,
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
            Descripcion = request.Descripcion,
            ImgUrl = returnedUrl.Value,
            PromedioEstrellas = 0,
            CantidadCalificaciones = 0,
            Confirmada = confirmada,
            UserId = userId,
            DateCreated = DateTime.UtcNow
        };

        dbContext.Comidas.Add(comida);
        await dbContext.SaveChangesAsync();

        return Result<Comida>.Ok(comida);
    }

    public async Task<Result<object>> RateComida(RateComidaRequestDto request, int comidaId, int userId)
    {
        var comida = await dbContext.Comidas.FindAsync(comidaId);

        if (comida == null)
            return Result<object>.Fail("La comida no existe");
        
        
        var sumatoriaCalificacionesAnterior = comida.PromedioEstrellas * comida.CantidadCalificaciones;
        comida.PromedioEstrellas = (sumatoriaCalificacionesAnterior + request.Rating) / (comida.CantidadCalificaciones + 1);
        comida.CantidadCalificaciones += 1;

        var newCalificacion = new Calificacion
        {
            Cantidad = request.Rating,
            UserId = userId,
            ComidaId = comida.Id
        };

        dbContext.Calificaciones.Add(newCalificacion);
        
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
        {
            return Result<object>.Fail("Ya califico esta comida");
        }

        return Result<object>.Ok(new { Success = true });
    }

    public async Task<Result<object>> UnrateComida(int comidaId, int userId)
    {
        var comida = await dbContext.Comidas.FindAsync(comidaId);

        if (comida == null)
            return Result<object>.Fail("La comida no existe");

        // Buscar la calificación del usuario para esta comida
        var calificacion = await dbContext.Calificaciones
            .FirstOrDefaultAsync(c => c.ComidaId == comidaId && c.UserId == userId);

        if (calificacion == null)
            return Result<object>.Fail("No tiene calificación en esta comida");

        // Calcular el nuevo promedio
        var sumatoriaSinLaCalificacion = comida.PromedioEstrellas * comida.CantidadCalificaciones - calificacion.Cantidad;
        
        if (comida.CantidadCalificaciones - 1 > 0)
        {
            comida.PromedioEstrellas = sumatoriaSinLaCalificacion / (comida.CantidadCalificaciones - 1);
        }
        else
        {
            // Si era la única calificación, resetear el promedio
            comida.PromedioEstrellas = 0;
        }
        
        comida.CantidadCalificaciones -= 1;

        // Eliminar la calificación
        dbContext.Calificaciones.Remove(calificacion);
        
        await dbContext.SaveChangesAsync();

        return Result<object>.Ok(new { Success = true });
    }

    public async Task<Result<object>> DeactivateComida(int comidaId, int userId)
    {
        var comida = await dbContext.Comidas.FindAsync(comidaId);

        if (comida == null)
            return Result<object>.Fail("La comida no existe");

        // Validar que el usuario sea el propietario de la comida
        if (comida.UserId != userId)
            return Result<object>.Fail("No tienes permisos para modificar esta comida");

        // Si ya está desactivada
        if (!comida.Activa)
            return Result<object>.Fail("La comida ya fue dada de baja");

        comida.Activa = false;
        await dbContext.SaveChangesAsync();

        return Result<object>.Ok(new { Success = true });
    }

    public async Task<Result<object>> DeleteComida(int comidaId, int userId)
    {
        var rowsAffected = await dbContext.Comidas
            .Where(c => c.Id == comidaId && c.UserId == userId)
            .ExecuteDeleteAsync();

        if (rowsAffected == 0)
            return Result<object>.Fail("La comida no existe o no tienes permisos");

        return Result<object>.Ok(new { Success = true });
    }

    public async Task<Result<object>> UpdateComida(UpdateComidaRequestDto request, int comidaId, int userId)
    {
        var comida = await dbContext.Comidas.FindAsync(comidaId);

        if (comida == null)
            return Result<object>.Fail("La comida no existe");

        // Validar que el usuario sea el propietario de la comida
        if (comida.UserId != userId)
            return Result<object>.Fail("No tienes permisos para modificar esta comida");

        if (string.IsNullOrEmpty(request.Titulo))
            return Result<object>.Fail("El titulo no puede estar vacio", field: "titulo");

        // Actualizar título y descripción
        comida.Titulo = request.Titulo;
        comida.Descripcion = request.Descripcion;

        // Si se proporciona una nueva imagen, actualizar
        if (request.File != null)
        {
            // Validar archivo
            var fileValidation = await fileService.VerifySingle(request.File);
            if (!fileValidation.Success) return Result<object>.Fail(fileValidation.Error);

            // Crear archivo
            var returnedUrl = await fileService.CreateFile(request.File, userId);
            if (!returnedUrl.Success) return Result<object>.Fail(returnedUrl.Error);

            comida.ImgUrl = returnedUrl.Value;
        }

        await dbContext.SaveChangesAsync();
        

        return Result<object>.Ok(new { Success = true });
    }    
}