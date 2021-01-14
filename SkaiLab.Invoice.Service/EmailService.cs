using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class EmailService:Service,IEmailService
    {
        IWebHostEnvironment webHostEnvironment;
        public EmailService(IDataContext context, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public string ReadEmailTemplate()
        {
            var templatePath= Path.Combine(webHostEnvironment.WebRootPath, "email-template.html");
            return File.ReadAllText(templatePath);
        }

        public async Task  SendEmailAsync(string toEmail, string subject, string body, bool isHtml)
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;
            message.Body = new TextPart(isHtml? "html" : "plain")
            {
                Text = body
            };
            await SendEmailAsync(message);

        }
        public async Task SendEmailAsync(MimeMessage message)
        {
            message.From.Add(new MailboxAddress(Option.MailKit.Email, Option.MailKit.Email));
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(Option.MailKit.Server, Option.MailKit.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(Option.MailKit.Email, Option.MailKit.Password);
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
        public async Task SendEmailAsync(List<string> toEmails, string subject, string body, bool isHtml)
        {
            var message = new MimeMessage();
            foreach (var toEmail in toEmails.Distinct())
            {
                message.To.Add(new MailboxAddress(toEmail, toEmail));
            }
            message.Subject = subject;
            message.Body = new TextPart(isHtml ? "html" : "plain")
            {
                Text = body
            };
            await SendEmailAsync(message);
        }


        public async Task SendEmailAsync(List<string> toEmails, string toName,string subject, string body, string bodyKh)
        {
            var bodyText = ReadEmailTemplate();
            bodyText = bodyText.Replace("[Name]", toName);
            bodyText = bodyText.Replace("[Body]", body);
            bodyText = bodyText.Replace("[BodyKH]", bodyKh);
            await SendEmailAsync(toEmails, subject, bodyText, true);
        }
    }
}
