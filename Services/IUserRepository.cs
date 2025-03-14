using Integrations.Model;
using Integrations.Model.Integrations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IUserRepository
    {
        Task<User> RegisterUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> VerifyUserAsync(int userId, bool isVerified);
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        string HashPassword(string password);
    }
}
