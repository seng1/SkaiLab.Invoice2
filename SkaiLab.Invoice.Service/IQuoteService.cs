using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IQuoteService:IService
    {
        List<Quote> GetQuotes(QuoteFilter filter);
        void GetTotalPages(QuoteFilter filter);
        List<QuoteStatus> GetQuoteStatuses(QuoteFilter filter);
        QuoteForUpdateOrCreate GetLookupForCreae(string organisationId);
        string GetQuoteNumber(string organisationId);
        void Create(Quote quote);
        QuoteForUpdateOrCreate GetForUpdate(string organisationId,long id);
        void Update(Quote quote);
        string UploadAttachemnt(long purchaseOrderId, string organisationId, string baseString,string fileName);
        void RemoveAttachment(long purchaseOrderId, string organisationId, string fileUrl);
        void Accept(List<long> quoteIds, string organisationId, string acceptBy);
        void Decline(List<long> quoteIds, string organisationId, string declineBy);
        void Delete(List<long> quoteIds, string organisationId, string deleteBy);
        List<StatusOverview> GetStatusOverviews(string organisationId);
        void ChangeOfficialDocument(long quoteId, string organisationId, string fileUrl);


    }
}
