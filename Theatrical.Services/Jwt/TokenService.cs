using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Theatrical.Data.Models;

namespace Theatrical.Services.Jwt;

public interface ITokenService
{
    string GenerateToken(User user);
}

public class TokenService : ITokenService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

    
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("ARandomKeyThatIsLikelyToGetChanged");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        var securityKey = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            SigningCredentials = signingCredentials,
            Issuer = "https://theatricalportal.azurewebsites.net",
            Audience = "https://theatricalportal.azurewebsites.net"
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
}