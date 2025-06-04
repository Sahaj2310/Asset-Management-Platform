using System;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AssetWeb.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _baseUrl;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["Email:SmtpServer"] ?? throw new InvalidOperationException("SMTP Server not configured");
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = configuration["Email:SmtpUsername"] ?? throw new InvalidOperationException("SMTP Username not configured");
            _smtpPassword = configuration["Email:SmtpPassword"] ?? throw new InvalidOperationException("SMTP Password not configured");
            _fromEmail = configuration["Email:FromEmail"] ?? throw new InvalidOperationException("From Email not configured");
            _fromName = configuration["Email:FromName"] ?? throw new InvalidOperationException("From Name not configured");
            _baseUrl = configuration["BaseUrl"] ?? "https://localhost:7038";
        }

        public async Task<bool> SendEmailConfirmationAsync(string email, Guid userId, string token)
        {
            var subject = "Confirm your email address";
            var confirmationLink = $"{_baseUrl}/api/Auth/confirm-email?userId={userId}&token={token}";
            var htmlContent = $@"
                <h2>Welcome to AssetWeb!</h2>
                <p>Please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>If you did not create an account, please ignore this email.</p>";

            return await SendEmailAsync(email, subject, htmlContent);
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlContent)
        {
            try
            {
                using var client = new SmtpClient(_smtpServer, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };
                message.To.Add(to);

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
