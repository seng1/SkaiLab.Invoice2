using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Quote
    {
        public Quote()
        {
            InvoiceQuote = new HashSet<InvoiceQuote>();
            QuoteAttachment = new HashSet<QuoteAttachment>();
            QuoteItem = new HashSet<QuoteItem>();
        }

        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string OrganisationId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string RefNo { get; set; }
        public string Number { get; set; }
        public string Note { get; set; }
        public int CurrencyId { get; set; }
        public decimal? TaxCurrencyExchangeRate { get; set; }
        public decimal? BaseCurrencyExchangeRate { get; set; }
        public decimal Total { get; set; }
        public decimal TotalIncludeTax { get; set; }
        public int StatusId { get; set; }
        public string CreatedBy { get; set; }
        public string AcceptedBy { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public string DeclinedBy { get; set; }
        public DateTime? DeclinedDate { get; set; }
        public string InvoicedBy { get; set; }
        public DateTime? InvoicedDate { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string TermAndCondition { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual QuoteStatus Status { get; set; }
        public virtual ICollection<InvoiceQuote> InvoiceQuote { get; set; }
        public virtual ICollection<QuoteAttachment> QuoteAttachment { get; set; }
        public virtual ICollection<QuoteItem> QuoteItem { get; set; }
    }
}
