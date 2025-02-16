using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);

    }
}
