using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using comidas_backend.Models;
using comidas_backend.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace comidas_backend.Services.Impl;

public class AuthServiceImpl(IOptions<AuthOptions> options) : IAuthService
{

    public Result<string> CreateToken(int id, string email, UserRole role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // var jwt = new JwtSecurityToken(is);
        var token = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(options.Value.ExpMinutes),
            notBefore: DateTime.UtcNow,
            signingCredentials: credentials
        );

        return Result<string>.Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
}