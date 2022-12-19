using CustomAuth.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace CustomAuth.Services
{
    public class EmailService : IDisposable
    {
        public string AdminEmail { get; }
        public string AdminPassword { get; }
        public SmtpConfig SmtpConfig { get; }
        private SmtpClient _client;
        public EmailService()
        {
            AdminEmail = ConfigurationManager.AppSettings["AdminEmail"];
            AdminPassword = ConfigurationManager.AppSettings["AdminPassword"];
            SmtpConfig = new SmtpConfig
            {
                Server = ConfigurationManager.AppSettings["SMTPServer"],
                Port = int.Parse(ConfigurationManager.AppSettings["SMTPPort"]),
                SslEnabled = bool.Parse(ConfigurationManager.AppSettings["SSL"])
            };
            _client = new SmtpClient();
        }

        public async Task SendAsync(MimeMessage message)
        {
            await _client.ConnectAsync(SmtpConfig.Server, SmtpConfig.Port, SmtpConfig.SslEnabled);
            await _client.AuthenticateAsync(AdminEmail, AdminPassword);
            await _client.SendAsync(message);
        }

        public void Dispose()
        {
            _client.Disconnect(true);
            _client.Dispose();
        }
    }
}
