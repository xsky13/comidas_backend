using comidas_backend.Models.Domain;
using Microsoft.EntityFrameworkCore;
namespace comidas_backend.Data;

public class ComidasDbContext : DbContext
{
    public ComidasDbContext(DbContextOptions<ComidasDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Comida> Comidas { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // asegura que la relacion entre la comida y el usuario sea unica (un usuario no puede tener mas de una calificacion para una comida)
        modelBuilder.Entity<Calificacion>()
            .HasIndex(c => new { c.ComidaId, c.UserId })
            .IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new Exception("Conexion a db no existe!"));
    }
}