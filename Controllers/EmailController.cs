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


       [HttpPost("{userContact}")]
        public async Task<IActionResult> ClinetContactToEmail(string userContact )
        {
            await _emailService.SendEmailAsync("lugedenidze@gmail.com","RentalInfo", $"{userContact} გამოუშვა შეკვეთა ბარის ქირაობასთან დაკავშირებით");
            return Ok("request is successfully!");
        }

        [HttpPost("{djReuqestContent}")]
        public async Task<IActionResult> DJRequest(string djReuqestContent)
        {
            await _emailService.SendEmailAsync("lugedenidze@gmail.com", "DJRequest", $"New Dj Request : {djReuqestContent} ");
            return Ok("request is successfully!");
        }


    }
}