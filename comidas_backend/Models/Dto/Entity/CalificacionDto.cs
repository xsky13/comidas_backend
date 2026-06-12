namespace comidas_backend.Models.Dto.Entity;

public class CalificacionDto
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public int UserId { get; set; }
    public int ComidaId { get; set; }
}