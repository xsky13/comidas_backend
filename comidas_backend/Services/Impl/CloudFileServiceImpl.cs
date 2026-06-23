using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using comidas_backend.Models.Dto;
using comidas_backend.Utils;

namespace comidas_backend.Services.Impl;

public class CloudFileServiceImpl : IFileService
{
    public async Task<Result<CloudFileCreationReturnDto>> CreateFile(IFormFile file, int userId)
    {
        if (file.Length == 0)
            return Result<CloudFileCreationReturnDto>.Fail("El archivo esta vacio");

        
        var cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL") ?? throw new Exception("No existe url"));
        cloudinary.Api.Secure = true;

        
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Folder = $"uploads/{userId}",
            UseFilename = true,
            UniqueFilename = true 
        };
        
        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            return Result<CloudFileCreationReturnDto>.Fail(uploadResult.Error.Message);

        return Result<CloudFileCreationReturnDto>.Ok(new CloudFileCreationReturnDto
        {
            Url = uploadResult.SecureUrl.ToString(),
            PublicID = uploadResult.PublicId
        });
    }

    public async Task<Result<bool>> DeleteFile(string fileName, int userId, string? publicId)
    {
        var cloudinary = new Cloudinary(
            Environment.GetEnvironmentVariable("CLOUDINARY_URL") 
            ?? throw new Exception("No existe url")
        );
        cloudinary.Api.Secure = true;
        
        var deleteParams = new DeletionParams(publicId);
        var result = await cloudinary.DestroyAsync(deleteParams);
        
        if (result.Result != "ok")
            return Result<bool>.Fail("No se pudo eliminar el archivo");
        
        return Result<bool>.Ok(true);
        // throw new NotImplementedException();
    }

    public async Task<Result<bool>> VerifySingle(IFormFile file)
    {
        if (file.Length > 10 * 1024 * 1024)
            return Result<bool>.Fail($"El archivo {file.FileName} excede el límite de 20MB.");

        var ext = Path.GetExtension(file.FileName).ToLower();

        var allowed = new[] {  ".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp",};

        if (!allowed.Contains(ext))
            return Result<bool>.Fail($"El tipo de archivo {ext} no está permitido.");

        return Result<bool>.Ok(true);
    }
}