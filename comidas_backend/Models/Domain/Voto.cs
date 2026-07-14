namespace comidas_backend.Models.Domain;

public class Voto
{
    public int Id { get; set; }
    public int VotoValue { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int ComentarioId { get; set; }
    public Comentario Comentario { get; set; }
}