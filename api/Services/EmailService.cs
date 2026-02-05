using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HRMS.api.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync([EmailAddress] string Recipient, string Subject, string Body );
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string Recipient, string Subject, string Body)
        {
            try
            {
                
            
                var message = new MimeMessage();
                message.From.Add(new  MailboxAddress(_configuration.GetSection("Email:FromName").Value, _configuration.GetSection("Email:FromAddress").Value));
                message.To.Add(new MailboxAddress("User", Recipient));
                message.Subject = Subject;
                message.Body = new TextPart("plain") { Text = Body };

                using var client = new SmtpClient();
                await client.ConnectAsync(_configuration.GetSection("Email:SmtpServer").Value,Int32.Parse(_configuration.GetSection("Email:Port").Value!), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration.GetSection("Email:Username").Value, _configuration.GetSection("Email:Password").Value);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email Sent Successfully to {Recipient}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Faild to send email to {Recipient}");
                throw;
            }
        }
    }
}

