using System.Threading.Tasks;

namespace Shared.Library.Messaging
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}
