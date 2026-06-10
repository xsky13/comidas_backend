namespace comidas_backend.Models;

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
}