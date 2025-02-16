using Integrations.Model.Integrations.Models;
using Integrations.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Integrations.Utils;
using System.Security.Claims;

namespace Integrations.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UserController(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            if (await _userRepository.GetUserByEmailAsync(user.Email) != null)
                return BadRequest(new { message = "Email already registered" });

            user.PasswordHash = HashPassword(user.PasswordHash); // ✅ Hash password before storing

            var newUser = await _userRepository.RegisterUserAsync(user);

            // ✅ Send verification pending email
            await _emailService.SendEmailAsync(user.Email, "Registration Successful", "Your verification is pending.");

            return CreatedAtAction(nameof(GetUserByEmail), new { email = newUser.Email }, newUser);
        }
        [Authorize]
        [HttpGet("{email}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email)?.Value;

            if ( userEmailClaim != email)
            {
                return BadRequest($"Unauthorized access attempt by {userEmailClaim}");
             }
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("verify/{userId}")]
        public async Task<IActionResult> VerifyUser(int userId, [FromQuery] bool isVerified)
        {
            var result = await _userRepository.VerifyUserAsync(userId, isVerified);

            if (!result) return NotFound(new { message = AppResourceHelper.Get("UserNotFound") });

            return Ok(new { message = isVerified ? AppResourceHelper.Get("UserVerifiedSuccess") : AppResourceHelper.Get("UserVerificationRejected") });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
