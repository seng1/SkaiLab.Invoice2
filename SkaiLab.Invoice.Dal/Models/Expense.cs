using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Expense
    {
        public Expense()
        {
            ExpenseAttachentFile = new HashSet<ExpenseAttachentFile>();
            ExpenseItem = new HashSet<ExpenseItem>();
        }

        public long Id { get; set; }
        public long VendorId { get; set; }
        public string OrganisationId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string RefNo { get; set; }
        public string Number { get; set; }
        public string Note { get; set; }
        public int CurrencyId { get; set; }
        public decimal? TaxCurrencyExchangeRate { get; set; }
        public decimal? BaseCurrencyExchangeRate { get; set; }
        public decimal Total { get; set; }
        public decimal TotalIncludeTax { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? BilledDate { get; set; }
        public string BilledBy { get; set; }
        public int ExpenseStatusId { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string TermAndCondition { get; set; }
        public DateTime? CloseDate { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual ExpenseStatus ExpenseStatus { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual Bill Bill { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual VendorExpense VendorExpense { get; set; }
        public virtual ICollection<ExpenseAttachentFile> ExpenseAttachentFile { get; set; }
        public virtual ICollection<ExpenseItem> ExpenseItem { get; set; }
    }
}
