using comidas_backend.Models.Domain;
using Microsoft.EntityFrameworkCore;
namespace comidas_backend.Data;

public class ComidasDbContext : DbContext
{
    public ComidasDbContext(DbContextOptions<ComidasDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Comida> Comidas { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }
    public DbSet<Voto> Votos { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // asegura que la relacion entre la comida y el usuario sea unica (un usuario no puede tener mas de una calificacion para una comida)
        modelBuilder.Entity<Calificacion>()
            .HasIndex(c => new { c.ComidaId, c.UserId })
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(u => u.Calificacions)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Comidas)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Comentarios)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Comida>()
            .HasMany(c => c.Comentarios)
            .WithOne(c => c.Comida)
            .HasForeignKey(c => c.ComidaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Comentario>()
            .HasMany(c => c.ListaVotos)
            .WithOne(v => v.Comentario)
            .HasForeignKey(v => v.ComentarioId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new Exception("Conexion a db no existe!"));
    }
}