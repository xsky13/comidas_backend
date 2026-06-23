namespace comidas_backend.Models.Domain;

public class Comida
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string? Descripcion { get; set; }
    public string ImgUrl { get; set; }
    public float PromedioEstrellas { get; set; }
    public int CantidadCalificaciones { get; set; }
    public bool Confirmada { get; set; }
    public bool Activa { get; set; } = true;
    public int? UserId { get; set; }
    public User User { get; set; }
    public List<Calificacion> Calificacions { get; set; }
    public DateTime DateCreated { get; set; }
    public string? PublicId { get; set; } = null;
}