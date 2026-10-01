using System.Security.Claims;
using comidas_backend.Controllers;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
using comidas_backend.Services;
using comidas_backend.Utils;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace comidas_backend.UnitTests;

public class ApiControllerTests
{
    private static UserController CreateUserController(IUserService userService, int userId = 42)
    {
        var controller = new UserController(userService);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuth"))
            }
        };

        return controller;
    }

    private static ComidaController CreateComidaController(IComidaService comidaService, int userId = 42)
    {
        var controller = new ComidaController(comidaService);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuth"))
            }
        };

        return controller;
    }

    private static PropuestaController CreatePropuestaController(IPropuestaService propuestaService, IComidaService comidaService, int userId = 7)
    {
        var controller = new PropuestaController(propuestaService, comidaService);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, "Admin")
                }, "TestAuth"))
            }
        };

        return controller;
    }

    [Fact]
    public async Task Register_ShouldAppendAccessTokenCookie_WhenRegistrationSucceeds()
    {
        // Arrange
        var userService = new Mock<IUserService>();
        userService
            .Setup(s => s.RegisterUser("Ana", "ana@test.com", "secret123"))
            .ReturnsAsync(Result<string>.Ok("jwt-token"));

        var controller = CreateUserController(userService.Object);

        // Act
        var actionResult = await controller.Register(new RegisterRequestDto
        {
            Nombre = "Ana",
            Email = "ana@test.com",
            Contrasena = "secret123"
        });

        // Assert
        actionResult.Should().BeOfType<OkResult>();
        controller.HttpContext.Response.Headers.SetCookie.ToString().Should().Contain("X-Access-Token");
        controller.HttpContext.Response.Headers.SetCookie.ToString().Should().Contain("jwt-token");
        userService.Verify(s => s.RegisterUser("Ana", "ana@test.com", "secret123"), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenCredentialsAreInvalid()
    {
        // Arrange
        var userService = new Mock<IUserService>();
        userService
            .Setup(s => s.LoginUser("bad@test.com", "wrongpass"))
            .ReturnsAsync(Result<string>.Fail("Credenciales inválidas", 401, "email"));

        var controller = CreateUserController(userService.Object);

        // Act
        var actionResult = await controller.Login(new LoginRequestDto
        {
            Email = "bad@test.com",
            Contrasena = "wrongpass"
        });

        // Assert
        actionResult.Should().BeOfType<BadRequestObjectResult>();
        controller.HttpContext.Response.Headers.SetCookie.ToString().Should().NotContain("X-Access-Token");
        userService.Verify(s => s.LoginUser("bad@test.com", "wrongpass"), Times.Once);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUserFromCurrentAuthenticatedUser()
    {
        // Arrange
        var userService = new Mock<IUserService>();
        var expectedUser = new UserDto
        {
            Id = 42,
            Nombre = "Carlos",
            Email = "carlos@test.com",
            Rol = UserRole.Admin
        };

        userService
            .Setup(s => s.GetUserById(42))
            .ReturnsAsync(Result<UserDto>.Ok(expectedUser));

        var controller = CreateUserController(userService.Object, 42);

        // Act
        var actionResult = await controller.GetUser();

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedUser);
        userService.Verify(s => s.GetUserById(42), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnFoodsForCurrentUser()
    {
        // Arrange
        var comidaService = new Mock<IComidaService>();
        var expectedFoods = new List<ComidaDto>
        {
            new() { Id = 1, Titulo = "Tacos", ImgUrl = "img-1.jpg", Confirmada = true, DateCreated = DateTime.UtcNow },
            new() { Id = 2, Titulo = "Enchiladas", ImgUrl = "img-2.jpg", Confirmada = true, DateCreated = DateTime.UtcNow }
        };

        comidaService
            .Setup(s => s.GetComidas(42))
            .ReturnsAsync(expectedFoods);

        var controller = CreateComidaController(comidaService.Object, 42);

        // Act
        var actionResult = await controller.GetAll();

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedFoods);
        comidaService.Verify(s => s.GetComidas(42), Times.Once);
    }

    [Fact]
    public async Task UpdateProposal_ShouldForwardCurrentUserIdentityAndTitleToService()
    {
        // Arrange
        var propuestaService = new Mock<IPropuestaService>();
        var comidaService = new Mock<IComidaService>();
        var updatedFood = new Comida
        {
            Id = 10,
            Titulo = "Nuevo título",
            ImgUrl = "updated.jpg",
            Confirmada = false,
            UserId = 7
        };

        propuestaService
            .Setup(s => s.UpdateProposal(7, "Admin", 10, "Nuevo título"))
            .ReturnsAsync(Result<Comida>.Ok(updatedFood));

        var controller = CreatePropuestaController(propuestaService.Object, comidaService.Object, 7);

        // Act
        var actionResult = await controller.UpdateProposal(new UpdateProposalRequestDto { Titulo = "Nuevo título" }, 10);

        // Assert
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        var okResult = actionResult.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(updatedFood);
        propuestaService.Verify(s => s.UpdateProposal(7, "Admin", 10, "Nuevo título"), Times.Once);
    }
}
