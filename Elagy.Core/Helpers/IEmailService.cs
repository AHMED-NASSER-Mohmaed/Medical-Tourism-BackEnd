using System.Threading.Tasks;

namespace Elagy.Core.Helpers
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}