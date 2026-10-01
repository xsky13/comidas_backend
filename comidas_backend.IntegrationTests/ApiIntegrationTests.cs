using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using comidas_backend.Data;
using comidas_backend.Models.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using FluentAssertions;

namespace comidas_backend.IntegrationTests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetComidas_ShouldReturnOnlyConfirmedAndActiveFoods()
    {
        await ResetDatabaseAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ComidasDbContext>();
            var user = new User { Nombre = "Laura", Email = "laura@test.com", PwdHash = "hash", Rol = UserRole.User };
            db.Users.Add(user);

            db.Comidas.AddRange(
                new Comida { Titulo = "Confirmed", Descripcion = "Visible", ImgUrl = "/img/visible.jpg", Confirmada = true, Activa = true, User = user, UserId = user.Id, DateCreated = DateTime.UtcNow.AddMinutes(-5) },
                new Comida { Titulo = "NotConfirmed", Descripcion = "Hidden", ImgUrl = "/img/hidden.jpg", Confirmada = false, Activa = true, User = user, UserId = user.Id, DateCreated = DateTime.UtcNow.AddMinutes(-4) },
                new Comida { Titulo = "Inactive", Descripcion = "Hidden", ImgUrl = "/img/inactive.jpg", Confirmada = true, Activa = false, User = user, UserId = user.Id, DateCreated = DateTime.UtcNow.AddMinutes(-3) }
            );

            await db.SaveChangesAsync();
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt(1, "laura@test.com", UserRole.User));

        var response = await _client.GetAsync("/api/Comida");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.ValueKind.Should().Be(JsonValueKind.Array);
        body.GetArrayLength().Should().Be(1);
        body[0].GetProperty("titulo").GetString().Should().Be("Confirmed");
    }

    [Fact]
    public async Task GetPropuestas_ShouldReturnOnlyPendingProposalsForCurrentUser()
    {
        await ResetDatabaseAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ComidasDbContext>();
            var user = new User { Nombre = "Miguel", Email = "miguel@test.com", PwdHash = "hash", Rol = UserRole.User };
            db.Users.Add(user);

            db.Comidas.AddRange(
                new Comida { Titulo = "Mi propuesta", Descripcion = "Pendiente", ImgUrl = "/img/pending.jpg", Confirmada = false, Activa = true, User = user, UserId = user.Id, DateCreated = DateTime.UtcNow.AddMinutes(-2) },
                new Comida { Titulo = "Otra propuesta", Descripcion = "Otra", ImgUrl = "/img/other.jpg", Confirmada = false, Activa = true, User = new User { Nombre = "Otro", Email = "otro@test.com", PwdHash = "hash", Rol = UserRole.User }, DateCreated = DateTime.UtcNow.AddMinutes(-1) },
                new Comida { Titulo = "Confirmada", Descripcion = "No es propuesta", ImgUrl = "/img/confirmed.jpg", Confirmada = true, Activa = true, User = user, UserId = user.Id, DateCreated = DateTime.UtcNow }
            );

            await db.SaveChangesAsync();
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt(1, "miguel@test.com", UserRole.User));

        var response = await _client.GetAsync("/api/Propuesta");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.ValueKind.Should().Be(JsonValueKind.Array);
        body.GetArrayLength().Should().Be(1);
        body[0].GetProperty("titulo").GetString().Should().Be("Mi propuesta");
    }

    [Fact]
    public async Task GetComidasByPromedio_ShouldReturnFoodsSortedDescendingByAverage()
    {
        await ResetDatabaseAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ComidasDbContext>();
            var user = new User { Nombre = "Nora", Email = "nora@test.com", PwdHash = "hash", Rol = UserRole.User };
            db.Users.Add(user);

            db.Comidas.AddRange(
                new Comida { Titulo = "Baja", PromedioEstrellas = 2.0f, CantidadCalificaciones = 2, Confirmada = true, Activa = true, User = user, UserId = user.Id, ImgUrl = "/img/baja.jpg", DateCreated = DateTime.UtcNow.AddMinutes(-2) },
                new Comida { Titulo = "Media", PromedioEstrellas = 4.5f, CantidadCalificaciones = 2, Confirmada = true, Activa = true, User = user, UserId = user.Id, ImgUrl = "/img/media.jpg", DateCreated = DateTime.UtcNow.AddMinutes(-1) },
                new Comida { Titulo = "Alta", PromedioEstrellas = 5.0f, CantidadCalificaciones = 2, Confirmada = true, Activa = true, User = user, UserId = user.Id, ImgUrl = "/img/alta.jpg", DateCreated = DateTime.UtcNow }
            );

            await db.SaveChangesAsync();
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt(1, "nora@test.com", UserRole.User));

        var response = await _client.GetAsync("/api/Comida/byPromedio?order=desc");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.ValueKind.Should().Be(JsonValueKind.Array);
        body.GetArrayLength().Should().Be(3);
        body[0].GetProperty("titulo").GetString().Should().Be("Alta");
        body[1].GetProperty("titulo").GetString().Should().Be("Media");
        body[2].GetProperty("titulo").GetString().Should().Be("Baja");
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ComidasDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    private static string CreateJwt(int userId, string email, UserRole role)
    {
        var key = Encoding.UTF8.GetBytes("SuperSecretKey1234567890SuperSecretKey1234567890");
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            },
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("Auth__Issuer", "test-issuer");
        Environment.SetEnvironmentVariable("Auth__Audience", "test-audience");
        Environment.SetEnvironmentVariable("Auth__Key", "SuperSecretKey1234567890SuperSecretKey1234567890");
        Environment.SetEnvironmentVariable("CONNECTION_STRING", "Host=localhost;Database=test;Username=test;Password=test");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ComidasDbContext>));
            services.RemoveAll<ComidasDbContext>();

            services.AddDbContext<ComidasDbContext>(options =>
            {
                options.UseInMemoryDatabase("ApiIntegrationTestsDb");
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ComidasDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
