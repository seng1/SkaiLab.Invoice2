using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IExpenseService:IService
    {
        string UploadAttachemnt(long purchaseOrderId, string organisationId, Attachment attachment);
        void RemoveAttachment(long purchaseOrderId, string organisationId, string fileUrl);
        void ChangeOfficialDocument(long purchaseOrderId, string organisationId, string fileUrl);
    }
}
