using comidas_backend.Models.Domain;

namespace comidas_backend.Models.Dto.Entity;

public class ComidaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string ImgUrl { get; set; }
    public float PromedioEstrellas { get; set; }
    public int CantidadCalificaciones { get; set; }
    public bool Confirmada { get; set; }
    public int UserId { get; set; }
    public IEnumerable<CalificacionDto> Calificaciones { get; set; } = [];
    public bool UsuarioCalifica { get; set; }
    public int? CalificacionUsuario { get; set; }
}