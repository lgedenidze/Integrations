using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Integrations.Data;
using Integrations.Model.Integrations.Models;
using Integrations.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Integrations.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> AuthenticateAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                return null; // Invalid credentials
            }

            return GenerateJwtToken(user);
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var enteredHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword)));
            return enteredHash == storedHash;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor

            {
                Subject = new ClaimsIdentity(new[]
                {
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Email, user.Email),
                 new Claim(ClaimTypes.Role, user.Role),
                 new Claim("FirstName", user.FirstName),
                 new Claim(ClaimTypes.GivenName, user.FirstName),
                 new Claim(ClaimTypes.Surname, user.LastName),
                 new Claim("IsVerified", user.IsVerified.ToString()),
                 new Claim("CreatedAt", user.CreatedAt.ToString("o")),
                 new Claim("Country", user.Country),
                 new Claim("PersonalNumber", user.PersonalNumber),
                 new Claim("Birthdate", user.Birthdate.ToString("o")),
                 new Claim("PhoneNumber", user.PhoneNumber ?? ""),
                 new Claim("SocialMediaProfileLink", user.SocialMediaProfileLink ?? ""),
                 new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
