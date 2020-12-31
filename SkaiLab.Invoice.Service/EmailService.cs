using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class EmailService:Service,IEmailService
    {
        public EmailService(IDataContext context) : base(context)
        {

        }

        public async Task  SendEmailAsync(string toEmail, string subject, string body, bool isHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("seng@skailab.com", "seng@skailab.com"));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;
            message.Body = new TextPart(isHtml? "html" : "plain")
            {
                Text = body
            };
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("seng@skailab.com", "Sako0009");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

        }
    }
}
