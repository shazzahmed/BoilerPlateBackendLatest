using Application.ServiceContracts;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Common.Options;
using System.Net;

namespace Infrastructure.Services.Communication
{
    public class EmailServicePlesk : IEmailService
    {
        private readonly GoogleOptions googleOptions;
        private string FromName = string.Empty;
        private string FromEmail = string.Empty;
        private string Username = string.Empty;
        private string Password = string.Empty;
        private string Host = string.Empty;
        private int Port;
        private bool EnableSsl;
        private bool UseDefaultCredentials;

        public EmailServicePlesk(IOptionsSnapshot<GoogleOptions> googleOptions)
        {
            this.googleOptions = googleOptions.Value;
            FromName = this.googleOptions.FromName;
            FromEmail = this.googleOptions.FromEmail;
            Username = this.googleOptions.Username;
            Password = this.googleOptions.Password;
            Host = this.googleOptions.Host;
            Port = this.googleOptions.Port;
            EnableSsl = this.googleOptions.EnableSsl;
            UseDefaultCredentials = this.googleOptions.UseDefaultCredentials;
        }

        public async Task<bool> SendEmail(string subject, string content, string toEmail, string fromEmail = null, string fromName = null, string attachment = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subject))
                    subject = FromName;
                if (string.IsNullOrWhiteSpace(fromName))
                    fromName = FromName;
                if (string.IsNullOrWhiteSpace(fromEmail))
                    fromEmail = FromEmail;
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;
                message.Body = new TextPart("HTML")
                {
                    Text = content
                };
                using (var client = new SmtpClient())
                {
                    // This bypasses SSL validation - NOT recommended for production!
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // Determine connection type based on port and SSL setting
                    SecureSocketOptions securityOptions;
                    if (EnableSsl && Port == 465)
                    {
                        // Port 465 = SSL/TLS
                        securityOptions = SecureSocketOptions.SslOnConnect;
                    }
                    else if (EnableSsl && (Port == 587 || Port == 25))
                    {
                        // Port 587/25 with SSL = STARTTLS (if supported)
                        securityOptions = SecureSocketOptions.StartTlsWhenAvailable;
                    }
                    else
                    {
                        // Port 25 without SSL = No encryption
                        securityOptions = SecureSocketOptions.None;
                    }

                    await client.ConnectAsync(Host, Port, securityOptions);
                    
                    // Only authenticate if username/password provided
                    if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                    {
                        await client.AuthenticateAsync(Username, Password);
                    }
                    
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                var test = ex;
                return false;
            }
            return true;
        }
    }
}
