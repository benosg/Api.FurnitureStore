using Api.FurnitureStore.API.Configuration;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Linq.Expressions;

namespace Api.FurnitureStore.API.Services
{
    public class EmailService : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
                message.To.Add(new MailboxAddress("",email));
                message.Subject = subject;
                message.Body= new TextPart(htmlMessage);
                using var client = new SmtpClient();

                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls).ConfigureAwait(false); ;
                await client.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password).ConfigureAwait(false); 
                await client.SendAsync(message).ConfigureAwait(false); 
                await client.DisconnectAsync(true);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
