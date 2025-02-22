using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:Sender"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendTicketEmailAsync(string toEmail, string eventName, string ticketId, string qrCodeUrl)
        {
            string subject = $"🎟 Your Ticket for {eventName}";
            string body = GetTicketEmailTemplate(eventName, ticketId, qrCodeUrl);

            await SendEmailAsync(toEmail, subject, body);
        }


        private string GetTicketEmailTemplate(string eventName, string ticketId, string qrCodeUrl)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px; }}
                        .email-container {{ max-width: 600px; background: white; padding: 20px; border-radius: 10px; text-align: center; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); }}
                        h2 {{ color: #333; }}
                        p {{ color: #555; }}
                        .qr-code img {{ width: 200px; margin-top: 10px; }}
                        .footer {{ margin-top: 20px; font-size: 12px; color: #888; }}
                    </style>
                </head>
                <body>
                    <div class='email-container'>
                        <h2>🎉 Your Ticket for {eventName} 🎉</h2>
                        <p>Ticket ID: <strong>{ticketId}</strong></p>
                        <p>Scan the QR code below at the event entrance:</p>
                        <div class='qr-code'>
                            <img src='{qrCodeUrl}' alt='QR Code'>
                        </div>
                        <p class='footer'>Thank you for purchasing with us! See you at the event. 🚀</p>
                    </div>
                </body>
                </html>
            ";
        }

    }

}
