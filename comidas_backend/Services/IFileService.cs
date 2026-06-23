using comidas_backend.Models.Dto;
using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface IFileService
{
    Task<Result<CloudFileCreationReturnDto>> CreateFile(IFormFile file, int userId);
    Task<Result<bool>> DeleteFile(string fileName, int userId, string? publicId);
    Task<Result<bool>> VerifySingle(IFormFile file);
}