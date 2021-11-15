using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Core.Helpers;
using Core.Interfaces.Common;

namespace Core.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }
        public async Task SendEmail(string toEmail, string toName, string subject, string body)
        {
            var message = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(_mailSettings.FromEmail, _mailSettings.FromName),
                Subject = subject,
                Body = body
            };
            message.To.Add(new MailAddress(toEmail, toName));

            var client = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
            {
                Credentials = new NetworkCredential(_mailSettings.Username, _mailSettings.Password),
                EnableSsl = true
            };
            await client.SendMailAsync(message);
        }
    }
}
