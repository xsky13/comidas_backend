namespace comidas_backend.Models.Dto.Entity;

public class ComentarioDto
{
    public int Id { get; set; }
    public string Texto { get; set; }
    public int Votos { get; set; }
    public DateTime Fecha { get; set; }
    public int ComidaId { get; set; }
    public int? UserId { get; set; }
    public UserDto? User { get; set; }
}