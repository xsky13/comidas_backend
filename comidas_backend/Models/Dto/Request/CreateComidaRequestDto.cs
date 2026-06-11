namespace comidas_backend.Models.Dto.Request;

public class CreateComidaRequestDto
{
    public string Titulo { get; set; }
    // public string ImgUrl { get; set; }
    public IFormFile File { get; set; }
}