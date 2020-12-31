using SkaiLab.Invoice.Dal.Models;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IInvoiceService:IService
    {
        Models.Invoice GetInvoiceFromQuote(long quoteId, string organsationId);
        string CreateInvoiceNumber(string organisationId, bool taxInvoice,int year, int month);
        string CreateInvoiceNumber( InvoiceContext context, string organisationId, bool taxInvoice, int year, int month);
        void CreateInvoiceFromQuote(Models.Invoice invoice);
        List<Models.Invoice> GetInvoices(InvoiceFilter filter);
        void GetTotalPages(InvoiceFilter filter);
        List<Models.InvoiceStatus> GetInvoiceStatuses(InvoiceFilter filter);
        void Create(Models.Invoice invoice);
        Models.Invoice GetInvoice(long id, string organisationId);
        List<StatusOverview> GetStatusOverviews(string organisationId);
        void Pay(long id, string organisationId, string paidBy);
        void Pay(List<long> id, string organisationId, string paidBy);
        void UploadFile(long id, string organisationId, Attachment attachment);
        void ChangeOfficialDocument(long id, string organisationId, string fileUrl);
    }
}
