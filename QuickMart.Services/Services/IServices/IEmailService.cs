using System.Threading.Tasks;

namespace QuickMart.Services.Services.IServices
{
    public interface IEmailService
    {
        #region Send Email Method

        Task SendEmailAsync(string recipientEmail, string subject, string body);

        #endregion
    }
}
