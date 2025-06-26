using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HobbyHub.UserIdentity.Provider.Abstractions;
using HobbyHub.UserIdentity.Provider.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HobbyHub.UserIdentity.Provider;

public class JwtService(IOptions<JwtConfig> options) : IJwtService
{
    private readonly JwtConfig _jwtConfig = options.Value;

    public string Generate(int userId, string email, string name, string surname, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.GivenName, name),
            new Claim(ClaimTypes.Surname, surname),
            new Claim(ClaimTypes.Role, role),
        };

        var token = new JwtSecurityToken(
            _jwtConfig.Issuer,
            _jwtConfig.Audience,
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool Verify(string jwt)
    {
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                var principal = tokenHandler.ValidateToken(jwt, validationParameters, out var validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}