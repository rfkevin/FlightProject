using FlightProject.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FlightProject.Services
{
    public class JwtService
    {
        private readonly JwtSetting _configuration;
        private readonly RSA _rsaPrivateKey;
        private readonly RSA _rsaPublicKey;

        public JwtService(IOptions<JwtSetting> optionsAccessor)
        {
            _configuration = optionsAccessor.Value;

            // Load the RSA private and public keys from the configuration
            string privateKeyPath = @"C:\Users\rfahe\OneDrive\Documents\INTECH\API\Securiter\private_key.pem";
            string publicKeyPath = @"C:\Users\rfahe\OneDrive\Documents\INTECH\API\Securiter\public_key.pem";

            // Create instances of RSA with the loaded private and public keys
            string privateKey = File.ReadAllText(privateKeyPath);
            _rsaPrivateKey = RSA.Create();
            _rsaPrivateKey.ImportFromEncryptedPem(privateKey, _configuration.password);
            
            string publicKey = File.ReadAllText(publicKeyPath);
            _rsaPublicKey = RSA.Create();
            _rsaPublicKey.ImportFromPem(publicKey);
            Console.WriteLine(_rsaPublicKey);
        }

            public string GenerateToken(Client user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Name, user.firstName),
                new Claim(ClaimTypes.Role, user.admin ? "admin" : "user")
            };

            var header = new JwtHeader(new SigningCredentials(new RsaSecurityKey(_rsaPrivateKey), SecurityAlgorithms.RsaSha256));

            var payload = new JwtPayload(_configuration.Issuer, _configuration.Audience, claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration.ExpirationMinutes)));

            var jwtToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(jwtToken);

            return tokenString;
        }
        public string GetUserIdFromToken(string tokenString)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                // Decode the token without validation
                var token = handler.ReadJwtToken(tokenString);

                // Extract the user ID claim
                var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}