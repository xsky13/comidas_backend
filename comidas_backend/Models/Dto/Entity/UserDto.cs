using comidas_backend.Models.Domain;

namespace comidas_backend.Models.Dto.Entity;

public class UserDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public UserRole Rol { get; set; }
}