using BaseCRUD.CookieAuth.Models;
using BaseCRUD.JWTAuth.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BaseCRUD.JWTAuth.Services
{
    public class JWTService
    {
        private readonly JWTSettings _settings;
        public JWTService(IOptions<JWTSettings> options)
        {
            _settings = options.Value;
        }
        public string GenerateToken(Users user)
        {
            List<Claim> claims = [
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.DisplayName)
                ];

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_settings.SecretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    issuer: _settings.Issuer,
                    audience: _settings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryInMinutes),
                    signingCredentials: cred
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
