using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ExpenseItem
    {
        public long Id { get; set; }
        public long ExpenseId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public double? DiscountRate { get; set; }
        public long? TaxId { get; set; }
        public decimal LineTotal { get; set; }
        public decimal LineTotalIncludeTax { get; set; }
        public string Description { get; set; }

        public virtual Expense Expense { get; set; }
        public virtual Tax Tax { get; set; }
        public virtual ExpenseProductItem ExpenseProductItem { get; set; }
    }
}
