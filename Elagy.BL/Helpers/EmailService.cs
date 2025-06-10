using Elagy.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System; // For Exception
using System.Net.Mail; // For SmtpClient, MailMessage
using System.Threading.Tasks;

namespace Elagy.BL.Helpers
{
    public class EmailService : IEmailService // Ensure it says IEmailService (your interface)
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpHost = _configuration["SmtpSettings:Host"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
            var smtpUsername = _configuration["SmtpSettings:Username"];
            var smtpPassword = _configuration["SmtpSettings:Password"];
            var fromEmail = _configuration["SmtpSettings:FromEmail"];
            var enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]);

            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail, "Elagy Platform");
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = true;

                    using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = enableSsl;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        await smtpClient.SendMailAsync(mail);
                    }
                }
                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, $"SMTP error sending email to {toEmail}: {smtpEx.Message}");
                if (smtpEx.InnerException != null)
                {
                    _logger.LogError(smtpEx.InnerException, $"Inner exception: {smtpEx.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {toEmail}: {ex.Message}");
            }
        }
    }
}