namespace comidas_backend.Models.Dto.Request;

public class UpdateUserRequestDto
{
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? Contrasena { get; set; }
}
