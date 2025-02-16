using Integrations.Data;
using Integrations.Model.Integrations.Models;
using Integrations.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService; // ✅ Add this field

        public UserRepository(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

}
}
