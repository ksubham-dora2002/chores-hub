using System.Threading.Tasks;

namespace ChoresHub.Application.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, string textBody);
    }
}
