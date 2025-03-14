using Integrations.Data;
using Integrations.Model.Integrations.Models;
using Integrations.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService; // ✅ Add this field
        private readonly IConfiguration _configuration;

        public UserRepository(AppDbContext context, IEmailService emailService ,  IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }


  public async Task<bool> VerifyUserAsync(int userId, bool isVerified)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsVerified = isVerified;
        await _context.SaveChangesAsync();

        // ✅ Fetch messages dynamically from `AppResource`
        string subject = AppResourceHelper.Get("EmailVerificationSubject");
        string message = isVerified
            ? AppResourceHelper.Get("UserVerifiedSuccess")
            : AppResourceHelper.Get("UserVerificationRejected");

        await _emailService.SendEmailAsync(user.Email, subject, message);

        return true;
    }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // Generate JWT as the reset token
            string token = GeneratePasswordResetToken(user.Email);

            // Send reset email
            string resetLink = $"https://yourfrontend.com/reset-password?token={token}";
            string subject = "Password Reset Request";
            string message = $"Click the link to reset your password: {resetLink}";

            await _emailService.SendEmailAsync(user.Email, subject, message);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            // Validate token and extract email
            string email = ValidatePasswordResetToken(token);
            if (email == null) return false;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // Hash the new password and update it
            user.Password = HashPassword(newPassword);

            await _context.SaveChangesAsync();
            return true;
        }

        private string GeneratePasswordResetToken(string email)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
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

        private string ValidatePasswordResetToken(string token)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No time delay allowed
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out _);
                return principal.FindFirstValue(ClaimTypes.Email); // Extract email from token
            }
            catch
            {
                return null; // Invalid token
            }
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }





    }
}
