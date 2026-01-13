using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ChoresHub.Application.Interfaces;

namespace ChoresHub.Infrastructure.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly string _senderName;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _host = configuration["EmailSettings:SmtpHost"] ?? "localhost";
            _port = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
            _senderEmail = configuration["EmailSettings:SenderEmail"] ?? "choreshubapp@gmail.com";
            _senderPassword = configuration["EmailSettings:SenderPassword"]!;
            _senderName = configuration["EmailSettings:SenderName"]!;
        }
        public async Task SendAsync(string toEmail, string subject, string htmlBody, string textBody)
        {   
            using var message = new MailMessage();
            message.From = new MailAddress(_senderEmail!, _senderName);
            message.To.Add(toEmail);
            message.Subject = subject;

            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            var textView = AlternateView.CreateAlternateViewFromString(textBody, null, "text/plain");

            message.AlternateViews.Add(textView);
            message.AlternateViews.Add(htmlView);

            using var client = new SmtpClient(_host, _port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_senderEmail, _senderPassword)
            };

            await client.SendMailAsync(message);
        }
    }
}
