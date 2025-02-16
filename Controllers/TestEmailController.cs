using System.Threading.Tasks;
using Integrations.Services;
using Microsoft.AspNetCore.Mvc;

namespace Integrations.Controllers
{
    [Route("api/test-email")]
    [ApiController]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendTestEmail([FromBody] string toEmail)
        {
            await _emailService.SendEmailAsync(toEmail, "Test Email", "This is a test email from your .NET app.");
            return Ok("Email sent successfully!");
        }
    }
}
