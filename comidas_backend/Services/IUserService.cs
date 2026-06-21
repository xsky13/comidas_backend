using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface IUserService
{
    Task<Result<string>> LoginUser(string email, string pwd);
    Task<Result<UserDto>> GetUserById(int id);
    Task<Result<string>> RegisterUser(string nombre, string email, string pwd);
    Task<Result<UserDto>> UpdateUser(int id, string? nombre, string? email, string? pwd); 
    Task<Result<UserDto>> ChangePassword(int userId, ChangePasswordRequestDto request); 
}