using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace ThoiTrangNam.Repository
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSSL
            };

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}
