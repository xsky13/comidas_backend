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
}