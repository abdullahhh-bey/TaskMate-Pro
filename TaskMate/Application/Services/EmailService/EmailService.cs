using MimeKit;
using MailKit.Net.Smtp;

namespace TaskMate.Application.Services.EmailService
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            _logger.LogInformation("Sent Email to {Email} at {Time}", toEmail, DateTime.UtcNow);

            // Make a message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config["Email:SenderName"], _config["Email:SenderEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            // Build an SMTP Connection
            using var client = new SmtpClient();
            await client.ConnectAsync(_config["Email:SmtpServer"], int.Parse(_config["Email:Port"]), false);
            await client.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
