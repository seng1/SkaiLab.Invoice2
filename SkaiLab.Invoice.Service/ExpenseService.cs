using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class ExpenseService:Service,IExpenseService
    {
        public ExpenseService(IDataContext context) : base(context)
        {

        }

        public void ChangeOfficialDocument(long purchaseOrderId, string organisationId, string fileUrl)
        {
            using var context = Context();
            var attachmentFiles = context.ExpenseAttachentFile.Where(u => u.ExpenseId == purchaseOrderId && u.Expense.OrganisationId == organisationId && u.FileUrl != fileUrl && u.IsFinalOfficalFile == true).ToList();
            foreach(var attachment in attachmentFiles)
            {
                attachment.IsFinalOfficalFile = false;
            }
            var t = context.ExpenseAttachentFile.FirstOrDefault(u => u.ExpenseId == purchaseOrderId && u.FileUrl == fileUrl);
            t.IsFinalOfficalFile = true;
            context.SaveChanges();
        }

        public void RemoveAttachment(long purchaseOrderId, string organisationId, string fileUrl)
        {
            using var context = Context();
            var expense = context.Expense.FirstOrDefault(u => u.Id == purchaseOrderId && u.OrganisationId == organisationId);
            if (expense == null)
            {
                throw new Exception("No exepnse");
            }
            if (expense.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
            {
                throw new Exception("Expense status doesn't allow to upload attachment.");
            }
            var attachmentFile = context.ExpenseAttachentFile.FirstOrDefault(u => u.ExpenseId == purchaseOrderId && u.Expense.OrganisationId == organisationId && u.FileUrl == fileUrl);
            if (attachmentFile != null)
            {
                context.ExpenseAttachentFile.Remove(attachmentFile);
                context.SaveChanges();
            }
        }

        public string UploadAttachemnt(long purchaseOrderId, string organisationId, Attachment attachment)
        {
            using var context = Context();
            var expense = context.Expense.FirstOrDefault(u => u.Id == purchaseOrderId && u.OrganisationId == organisationId);
            if (expense == null)
            {
                throw new Exception("No exepnse");
            }
            if (expense.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
            {
                throw new Exception("Expense status doesn't allow to upload attachment.");
            }
            var fileUrl = UploadFile(attachment.FileUrl,attachment.FileName);
            expense.ExpenseAttachentFile.Add(new Dal.Models.ExpenseAttachentFile
            {
                FileUrl=fileUrl,
                FileName=attachment.FileName,
                IsFinalOfficalFile=attachment.IsFinalOfficalFile
            });
            context.SaveChanges();
            return fileUrl;
        }
    }
}
