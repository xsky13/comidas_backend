namespace comidas_backend.Models.Domain;

public class Comentario
{
    public int Id { get; set; }
    public string Texto { get; set; }
    public int Votos { get; set; }
    public DateTime Fecha { get; set; }
    public int ComidaId { get; set; }
    public Comida Comida { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public List<Voto> ListaVotos { get; set; }
}