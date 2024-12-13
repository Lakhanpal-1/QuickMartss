using QuickMart.Services.Services.IServices;
using System.Net;
using System.Net.Mail;

namespace QuickMart.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly string smtpServer;
        private readonly int port;
        private readonly string senderEmail;
        private readonly string senderPassword;

        #region Constructor

        public EmailService(string smtpServer, int port, string senderEmail, string senderPassword)
        {
            this.smtpServer = smtpServer;
            this.port = port;
            this.senderEmail = senderEmail;
            this.senderPassword = senderPassword; // App password comes from the configuration
        }

        #endregion

        #region Send Email Method

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(smtpServer, port))
                {
                    client.Credentials = new NetworkCredential(senderEmail, senderPassword); // Use the App password here
                    client.EnableSsl = true; // Ensure SSL is enabled for secure connection

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true // Set the body as HTML
                    };

                    mailMessage.To.Add(recipientEmail); // Add recipient email
                    await client.SendMailAsync(mailMessage); // Send the email asynchronously
                }
            }
            catch (Exception ex)
            {
                // Log the exception or rethrow based on your logging strategy
                throw new InvalidOperationException("An error occurred while sending the email", ex);
            }
        }

        #endregion
    }
}
