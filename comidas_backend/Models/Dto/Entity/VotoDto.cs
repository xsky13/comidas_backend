namespace comidas_backend.Models.Dto.Entity;

public class VotoDto
{
    public int Id { get; set; }
    public int VotoValue { get; set; }
    public int UserId { get; set; }
    public int ComentarioId { get; set; }
}