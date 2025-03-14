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
using Integrations.Model;
using System.Linq;

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

            user.Password = _userRepository.HashPassword(user.Password); // ✅ Hash password before storing

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
            var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value; // Optional: Check for Admin role

            if (userRoleClaim != "Admin" && userEmailClaim != email)
            {
                return BadRequest($"Unauthorized access attempt by {userEmailClaim}");
            }
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("verify/{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> VerifyUser(int userId, [FromQuery] bool isVerified)
        {
            var result = await _userRepository.VerifyUserAsync(userId, isVerified);

            if (!result) return NotFound(new { message = AppResourceHelper.Get("UserNotFound") });

            return Ok(new { message = isVerified ? AppResourceHelper.Get("UserVerifiedSuccess") : AppResourceHelper.Get("UserVerificationRejected") });
        }




        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetCurrentUser()
        {
            var userClaims = User.Claims.ToList();
            var userDto = new UserDto();
            if (userClaims.Count > 0) { 



                userDto = new UserDto
                {
                    Id = int.Parse(userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0"),
                    Email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    Role = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                    IsVerified = bool.Parse(userClaims.FirstOrDefault(c => c.Type == "IsVerified")?.Value ?? "false"),
                    FirstName = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                    LastName = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
                    CreatedAt = DateTime.Parse(userClaims.FirstOrDefault(c => c.Type == "CreatedAt")?.Value),
                    Country = userClaims.FirstOrDefault(c => c.Type == "Country")?.Value,
                    PersonalNumber = userClaims.FirstOrDefault(c => c.Type == "PersonalNumber")?.Value,
                    Birthdate = DateTime.Parse(userClaims.FirstOrDefault(c => c.Type == "Birthdate")?.Value),
                    PhoneNumber = userClaims.FirstOrDefault(c => c.Type == "PhoneNumber")?.Value,
                    SocialMediaProfileLink = userClaims.FirstOrDefault(c => c.Type == "SocialMediaProfileLink")?.Value,
                };

        } 


            return Ok(userDto);

        }

        /// <summary>
        /// Request password reset by sending an email with a reset link.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("request-password-reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            var success = await _userRepository.RequestPasswordResetAsync(email);
            if (!success) return NotFound("User not found.");
            return Ok("Password reset link has been sent.");
        }

        /// <summary>
        /// Reset password using the provided token and new password.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var success = await _userRepository.ResetPasswordAsync(request.Token, request.NewPassword);
            if (!success) return BadRequest("Invalid or expired token.");
            return Ok("Password has been reset successfully.");
        }
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}

