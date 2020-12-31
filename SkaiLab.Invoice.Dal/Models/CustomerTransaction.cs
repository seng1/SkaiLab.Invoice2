using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class CustomerTransaction
    {
        public CustomerTransaction()
        {
            CustomerTransactionAttachment = new HashSet<CustomerTransactionAttachment>();
            CustomerTransactionItem = new HashSet<CustomerTransactionItem>();
        }

        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string OrganisationId { get; set; }
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
        public string TermAndCondition { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual ICollection<CustomerTransactionAttachment> CustomerTransactionAttachment { get; set; }
        public virtual ICollection<CustomerTransactionItem> CustomerTransactionItem { get; set; }
    }
}
