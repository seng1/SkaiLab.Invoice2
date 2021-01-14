using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IEmailService:IService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml);
        string ReadEmailTemplate();
        Task SendEmailAsync(List<string> toEmails, string toName, string subject, string body, string bodyKh);
    }
}
