using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface IPropuestaService
{
    Task<List<ComidaDto>> GetProposals(int userId, string userRole);
    Task<Result<Comida>> UpdateProposal(int userId, string userRole, int comidaId, string newTitle);
    Task<Result<Comida>> DeleteProposal(int userId, string userRole, int comidaId);

}