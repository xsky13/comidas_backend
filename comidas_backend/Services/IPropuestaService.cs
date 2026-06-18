using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface IPropuestaService
{
    Task<List<ComidaDto>> GetProposals(int userId);
    Task<Result<Comida>> UpdateProposal(int userId, string userRole, int comidaId, string newTitle);
}