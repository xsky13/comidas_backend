using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface IComidaService
{
    Task<List<ComidaDto>> GetComidas(int userId);
    Task<List<ComidaDto>> GetComidasAsc(int userId);
    Task<List<ComidaDto>> GetComidasDesc(int userId);
    Task<Result<Comida>> CreateComida(CreateComidaRequestDto request, bool confirmada, int userId);
    Task<Result<object>> RateComida(RateComidaRequestDto request, int comidaId, int userId);
    Task<Result<object>> UnrateComida(int comidaId, int userId);
    Task<Result<object>> DeactivateComida(int comidaId, int userId);
    Task<Result<object>> DeleteComida(int comidaId, int userId, string userRole);
    Task<Result<object>> UpdateComida(UpdateComidaRequestDto request, int comidaId, int userId);
}