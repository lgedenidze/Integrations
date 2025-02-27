using System.Threading.Tasks;
using Integrations.Services;
using Microsoft.AspNetCore.Mvc;

namespace Integrations.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("ClientContactToEmail")]
        public async Task<IActionResult> ClientContactToEmail([FromBody] string userContact)
        {
            if (string.IsNullOrWhiteSpace(userContact))
            {
                return BadRequest("User contact information is required.");
            }

            await _emailService.SendEmailAsync(
                "lugedenidze@gmail.com",
                "RentalInfo",
                $"{userContact} has requested information about renting a bar."
            );

            return Ok("Request was successful!");
        }

        [HttpPost("DJRequest")]
        public async Task<IActionResult> DJRequest([FromBody] string djRequestContent)
        {
            if (string.IsNullOrWhiteSpace(djRequestContent))
            {
                return BadRequest("DJ request content is required.");
            }

            await _emailService.SendEmailAsync(
                "lugedenidze@gmail.com",
                "DJRequest",
                $"New DJ Request: {djRequestContent}"
            );

            return Ok("Request was successful!");
        }
    }
}
