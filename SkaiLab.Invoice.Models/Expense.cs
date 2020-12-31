using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Models
{
    public class Expense
    {
        public long Id { get; set; }
        public long VendorId { get; set; }
        public string OrganisationId { get; set; }
        public Vendor Vendor { get; set; }
        public DateTime Created { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string OrderNumber { get; set; }
        public string Note { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public decimal TaxCurrencyExchangeRate { get; set; }
        public decimal BaseCurrencyExchangeRate { get; set; }
        public decimal Total { get; set; }
        public decimal TotalIncludeTax { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? BilledDate { get; set; }
        public string BilledBy { get; set; }
        public string CustomerInvoiceUrl { get; set; }
        public int ExpenseStatusId { get; set; }
        public ExpenseStatus ExpenseStatus { get; set; }
        public string RefNo { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public List<Attachment> Attachments { get; set; }
        public List<PurchaseOrderItem> ExpenseItems { get; set; }
        public Organsation Organisation { get; set; }
        public string TermAndCondition { get; set; }
        public DateTime? CloseDate { get; set; }
        public bool HasCloseDoc { get; set; }
        public Attachment CloseAttachment { get; set; }
    }
    public class ExpenseForUpdate
    {
        public Expense Expense { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<Location> Locations { get; set; }
        public List<Tax> Taxes { get; set; }
        public int BaseCurrencyId { get; set; }
        public Currency TaxCurrency { get; set; }

    }
    public class ExpenseStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Count { get; set; }
    }
    public class ExpenseItem
    {
        public long Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public double? DiscountRate { get; set; }
        public long? TaxId { get; set; }
        public Tax Tax { get; set; }
        public decimal LineTotal { get; set; }
        public decimal LineTotalIncludeTax { get; set; }
        public string Description { get; set; }
        public long ExpenseId { get; set; }
    }
}
