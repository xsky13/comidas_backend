namespace comidas_backend.Models.Domain;

public enum UserRole
{
    Admin = 1,
    User = 2
}

public class User
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string PwdHash { get; set; }
    public UserRole Rol { get; set; }
    public List<Calificacion> Calificacions { get; set; }
    public List<Comida> Comidas { get; set; }
    public List<Comentario> Comentarios { get; set; }
}