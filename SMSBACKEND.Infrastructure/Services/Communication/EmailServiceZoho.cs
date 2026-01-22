using Microsoft.Extensions.Options;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Common.Options;
using Application.ServiceContracts;

namespace Infrastructure.Services.Communication
{
    public class EmailServiceZoho : IEmailService
    {
        private readonly ZohoOptions zohoOptions;
        private string FromName = string.Empty;
        private string FromEmail = string.Empty;
        private string Username = string.Empty;
        private string Password = string.Empty;
        private string Host = string.Empty;
        private int Port;
        private bool EnableSsl;
        private bool UseDefaultCredentials;

        public EmailServiceZoho(IOptionsSnapshot<ZohoOptions> zohoOptions)
        {
            this.zohoOptions = zohoOptions.Value;
            FromName = this.zohoOptions.FromName;
            FromEmail = this.zohoOptions.FromEmail;
            Username = this.zohoOptions.Username;
            Password = this.zohoOptions.Password;
            Host = this.zohoOptions.Host;
            Port = this.zohoOptions.Port;
            EnableSsl = this.zohoOptions.EnableSsl;
            UseDefaultCredentials = this.zohoOptions.UseDefaultCredentials;
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
                //var contentresult = "We are excited to have you get started in ITSolution.First,you need to confirm your account just click below this link";
                var mailMsg = new MailMessage()
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = content,
                    IsBodyHtml = true
                };

                if (!string.IsNullOrEmpty(attachment))
                    mailMsg.Attachments.Add(new Attachment(attachment));

                mailMsg.To.Add(toEmail);

                SmtpClient smtpClient = new SmtpClient
                {
                    Host = Host,
                    Port = Port,
                    EnableSsl = EnableSsl,
                    UseDefaultCredentials = UseDefaultCredentials,
                    Credentials = new System.Net.NetworkCredential(Username, Password)
                };
                smtpClient.Send(mailMsg);
                mailMsg.Dispose();
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
