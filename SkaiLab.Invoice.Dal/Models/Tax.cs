using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Tax
    {
        public Tax()
        {
            CustomerTransactionItem = new HashSet<CustomerTransactionItem>();
            ExpenseItem = new HashSet<ExpenseItem>();
            ProductPurchaseInformation = new HashSet<ProductPurchaseInformation>();
            ProductSaleInformation = new HashSet<ProductSaleInformation>();
            QuoteItem = new HashSet<QuoteItem>();
            TaxComponent = new HashSet<TaxComponent>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual ICollection<CustomerTransactionItem> CustomerTransactionItem { get; set; }
        public virtual ICollection<ExpenseItem> ExpenseItem { get; set; }
        public virtual ICollection<ProductPurchaseInformation> ProductPurchaseInformation { get; set; }
        public virtual ICollection<ProductSaleInformation> ProductSaleInformation { get; set; }
        public virtual ICollection<QuoteItem> QuoteItem { get; set; }
        public virtual ICollection<TaxComponent> TaxComponent { get; set; }
    }
}
