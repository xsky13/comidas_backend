using comidas_backend.Data;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace comidas_backend.Services.Impl;

public class UserServiceImpl(ComidasDbContext dbContext, IAuthService authService) : IUserService
{
    public async Task<Result<string>> LoginUser(string email, string pwd)
    {
        // find user with email
        var userWithEmail = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);

        if (userWithEmail == null)
            return Result<string>.Fail("El usuario con ese email no existe.", field: "email");

        // compare hash
        if (!BCrypt.Net.BCrypt.Verify(pwd, userWithEmail.PwdHash))
            return Result<string>.Fail("Contrasena incorrecta.", field: "contrasena");
        
        // return token
        var token = authService.CreateToken(userWithEmail.Id, userWithEmail.Email, userWithEmail.Rol);
        return Result<string>.Ok(token.Value!);
    }

    public async Task<Result<UserDto>> GetUserById(int id)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user == null) return Result<UserDto>.Fail("El usuario no existe");

        return Result<UserDto>.Ok(new UserDto()
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        });
    }
        public async Task<Result<string>> RegisterUser(string nombre, string email, string pwd)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result<string>.Fail("El nombre es obligatorio.", field: "nombre");

        if (string.IsNullOrWhiteSpace(email))
            return Result<string>.Fail("El email es obligatorio.", field: "email");

        if (string.IsNullOrWhiteSpace(pwd) || pwd.Length < 6)
            return Result<string>.Fail("La contraseña debe tener al menos 6 caracteres.", field: "contrasena");

        if (await dbContext.Users.AnyAsync(user => user.Email == email))
            return Result<string>.Fail("Ya existe un usuario con ese email.", field: "email");

        var newUser = new Models.User
        {
            Nombre = nombre,
            Email = email,
            PwdHash = BCrypt.Net.BCrypt.HashPassword(pwd),
            Rol = Models.UserRole.User
        };

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        var token = authService.CreateToken(newUser.Id, newUser.Email, newUser.Rol);
        return Result<string>.Ok(token.Value!);
    }
}