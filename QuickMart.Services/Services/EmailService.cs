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

        // Constructor to initialize SMTP details
        public EmailService(string smtpServer, int port, string senderEmail, string senderPassword)
        {
            this.smtpServer = smtpServer;
            this.port = port;
            this.senderEmail = senderEmail;
            this.senderPassword = senderPassword; // App password comes from the configuration
        }

        // Method to send the email asynchronously
        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
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
    }
}
