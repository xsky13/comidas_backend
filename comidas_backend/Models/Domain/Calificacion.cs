namespace comidas_backend.Models.Domain;

public class Calificacion
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int ComidaId { get; set; }
    public Comida Comida { get; set; }
}