using FlightProject.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FlightProject.Services
{
    public class JwtService
    {
        private readonly JwtSettign _configuration;
        public JwtService (IOptions<JwtSettign> options)
        {
            _configuration = options.Value;
        }
        public string GenerateToken(Client user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.admin ? "admin" : "user")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration.ExpirationMinutes));
            var token = new JwtSecurityToken(_configuration.Issuer, _configuration.Audience, claims, expires: expires, signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
 