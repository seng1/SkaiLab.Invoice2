using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Models
{
    public class CustomerTransaction
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string OrganisationId { get; set; }
        public int StatusId { get; set; }
        public string RefNo { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public int CurrencyId { get; set; }
        public decimal TaxCurrencyExchangeRate { get; set; }
        public decimal BaseCurrencyExchangeRate { get; set; }
        public decimal Total { get; set; }
        public decimal TotalIncludeTax { get; set; }
        public bool IsTaxIncome { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string PaidBy { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Number { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Customer Customer { get; set; }
        public string TermAndCondition { get; set; }
      
        public List<Attachment> Attachments { get; set; }
        public  List<CustomerTransactionItem> CustomerTransactionItems { get; set; }
        public Organsation Organisation { get; set; }
    }
    public  class CustomerTransactionItem
    {
        public long Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public double? DiscountRate { get; set; }
        public long? TaxId { get; set; }
        public decimal LineTotal { get; set; }
        public decimal LineTotalIncludeTax { get; set; }
        public string Description { get; set; }
        public long? ProductId { get; set; }
        public long? LocationId { get; set; }
        public  Location Location { get; set; }
        public  Product Product { get; set; }
        public Tax Tax { get; set; }
        public long CustomerTransaction { get; set; }

    }
    public class Invoice:CustomerTransaction
    {
        public InvoiceStatus Status { get; set; }
        public long? QuoteId { get; set; }
    }
    public class InvoiceStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
       
    }
    public class CustomerCreditStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

    }
    public class CustomerCredit : CustomerTransaction
    {
        public CustomerCreditStatus Status { get; set; }
    }
}
