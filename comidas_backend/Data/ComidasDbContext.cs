using comidas_backend.Models;
using Microsoft.EntityFrameworkCore;
namespace comidas_backend.Data;

public class ComidasDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new Exception("Conexion a db no existe!"));
    }
}