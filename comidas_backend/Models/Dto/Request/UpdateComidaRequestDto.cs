namespace comidas_backend.Models.Dto.Request;

public class UpdateComidaRequestDto
{
    public string Titulo { get; set; }
    public string? Descripcion { get; set; }
    public IFormFile? File { get; set; }
}
