using System.Threading.Tasks;

namespace Core.Interfaces.Common
{
    public interface IMailService
    {
        Task SendEmail(string toEmail, string toName, string subject, string body);
    }
}
