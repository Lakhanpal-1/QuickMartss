using System.Threading.Tasks;

namespace QuickMart.Services.Services.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string body);
    }
}
