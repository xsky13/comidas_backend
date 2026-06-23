using comidas_backend.Data;
using comidas_backend.Models.Domain;
using comidas_backend.Models.Dto.Entity;
using comidas_backend.Models.Dto.Request;
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
        if (user == null) return Result<UserDto>.Fail("El usuario no existe fuap 4");

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

        var newUser = new User
        {
            Nombre = nombre,
            Email = email,
            PwdHash = BCrypt.Net.BCrypt.HashPassword(pwd),
            Rol = UserRole.User
        };

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        var token = authService.CreateToken(newUser.Id, newUser.Email, newUser.Rol);
        return Result<string>.Ok(token.Value!);
    }

    public async Task<Result<UserDto>> UpdateUser(int id, string? nombre, string? email, string? pwd)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user == null) return Result<UserDto>.Fail("El usuario no existe fuap 3");

        if (!string.IsNullOrWhiteSpace(nombre))
            user.Nombre = nombre;

        if (!string.IsNullOrWhiteSpace(email))
        {
            if (await dbContext.Users.AnyAsync(u => u.Email == email && u.Id != id))
                return Result<UserDto>.Fail("Ya existe un usuario con ese email.", field: "email");
            user.Email = email;
        }

        if (!string.IsNullOrWhiteSpace(pwd))
        {
            if (pwd.Length < 6)
                return Result<UserDto>.Fail("La contraseña debe tener al menos 6 caracteres.", field: "contrasena");
            user.PwdHash = BCrypt.Net.BCrypt.HashPassword(pwd);
        }

        await dbContext.SaveChangesAsync();

        return Result<UserDto>.Ok(new UserDto()
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        });
    }

    public async Task<Result<UserDto>> ChangePassword(int userId, ChangePasswordRequestDto request)
    {
        var user = await dbContext.Users.FindAsync(userId);
        if (user == null) return Result<UserDto>.Fail("El usuario no existe fuap 2");
        
        // el usuario es obtenido con el token, asi que eso ya verifica que sea el usuario autenticado
        if (!BCrypt.Net.BCrypt.Verify(request.oldPassword, user.PwdHash))
            return Result<UserDto>.Fail("La contrasena antigua no es valida", field: "oldPassword");
        
        if (request.newPassword != request.newPasswordRepeat)
            return Result<UserDto>.Fail("Las contrasenas no coinciden", field: "newPasswordRepeat");

        user.PwdHash = BCrypt.Net.BCrypt.HashPassword(request.newPassword);
        await dbContext.SaveChangesAsync();

        return Result<UserDto>.Ok(new UserDto
        {
            Id = userId,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        });
    }

    public async Task<Result<string>> DeleteUser(int id)
    {
        var RowsAffected = await dbContext.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
        return  Result<string>.Ok("Usuario eliminado correctamente") ;
    }
}