using comidas_backend.Models.Domain;
using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface IAuthService
{
    Result<string> CreateToken(int id, string email, UserRole role);
}