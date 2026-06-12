using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface IComidaService
{
    Task<List<ComidaDto>> GetComidas(int userId);
    Task<Result<Comida>> CreateComida(CreateComidaRequestDto request, bool confirmada, int userId);
}