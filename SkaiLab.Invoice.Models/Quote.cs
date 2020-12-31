using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Models
{
    public class Quote
    {
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

        public virtual Currency Currency { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual QuoteStatus Status { get; set; }
        public List<QuoteItem> QuoteItems { get; set; }
        public List<Attachment> Attachments { get; set; }
        public Organsation Organisation {get;set;}
        public string TermAndCondition { get; set; }
    }
    public class QuoteStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class QuoteItem
    {
        public long Id { get; set; }
        public long QuoteId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public double? DiscountRate { get; set; }
        public long? TaxId { get; set; }
        public decimal LineTotal { get; set; }
        public decimal LineTotalIncludeTax { get; set; }
        public string Description { get; set; }
        public long ProductId { get; set; }
        public long? LocationId { get; set; }

        public virtual Location Location { get; set; }
        public virtual Product Product { get; set; }
        public virtual Tax Tax { get; set; }
    }

    public class QuoteForUpdateOrCreate
    {
        public Quote Quote { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<Location> Locations { get; set; }
        public List<Tax> Taxes { get; set; }
        public int BaseCurrencyId { get; set; }
        public Currency TaxCurrency { get; set; }
        public string Number { get; set; }

    }
}
