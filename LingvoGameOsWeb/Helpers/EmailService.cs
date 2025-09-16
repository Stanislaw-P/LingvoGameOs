using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Plugins;

namespace LingvoGameOs.Helpers
{
    public class EmailService
    {
        private readonly MailSettings mailSettings;
        public EmailService(IOptions<MailSettings> mailSettings)
        {
            this.mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            emailMessage.To.Add(new MailboxAddress(mailSettings.DisplayName, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(mailSettings.Host, mailSettings.Port);
                await client.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
