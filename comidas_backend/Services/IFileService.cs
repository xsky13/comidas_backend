using comidas_backend.Utils;

namespace comidas_backend.Services;

public interface IFileService
{
    Task<Result<string>> CreateFile(IFormFile file, int userId);
    Task<Result<bool>> DeleteFile(string fileName, int userId);
    Task<Result<bool>> VerifySingle(IFormFile file);
}