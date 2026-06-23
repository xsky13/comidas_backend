using comidas_backend.Utils;

namespace comidas_backend.Services.Impl;

public class LocalFileServiceImpl : IFileService
{
    private readonly string path = "/home/jared/comidas_backend/comidas_backend/UploadAreaTemp";
    public async Task<Result<string>> CreateFile(IFormFile file, int userId)
    {
        try
        {
            string fileName = "";
            if (file.Length > 0)
            {
                var userPath = Path.Combine(path, userId.ToString());

                if (!Directory.Exists(userPath))
                    Directory.CreateDirectory(userPath);

                fileName = Path.GetFileName(file.FileName);
                string fullPath = Path.Combine(userPath, fileName);
                if (File.Exists(fullPath))
                {
                    fileName = $"{DateTime.Now.Ticks}_${file.FileName}";
                    fullPath = Path.Combine(userPath, fileName);
                }


                using (var stream = System.IO.File.Create(fullPath))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Result<string>.Ok($"http://localhost:5125/uploads/{userId}/{fileName}");
        }
        catch (Exception e)
        {
            return Result<string>.Fail(e.Message);
        }
    }

    public async Task<Result<bool>> DeleteFile(string fileName, int userId)
    {
        var filePath = Path.Combine(path, $"{userId}/{Path.GetFileName(fileName)}");
        if (System.IO.File.Exists(filePath))
        {
            try
            {
                System.IO.File.Delete(filePath);
                return Result<bool>.Ok(true);
            }
            catch (IOException ex)
            {
                return Result<bool>.Fail(ex.Message);
            }
        }
        else
        {
            return Result<bool>.Fail("El archivo no existe");
        }
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