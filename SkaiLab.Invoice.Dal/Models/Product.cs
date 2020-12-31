using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Product
    {
        public Product()
        {
            CustomerTransactionItemProduct = new HashSet<CustomerTransactionItemProduct>();
            ExpenseProductItem = new HashSet<ExpenseProductItem>();
            QuoteItem = new HashSet<QuoteItem>();
        }

        public long Id { get; set; }
        public string OrganisationId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual ProductInventory ProductInventory { get; set; }
        public virtual ProductPurchaseInformation ProductPurchaseInformation { get; set; }
        public virtual ProductSaleInformation ProductSaleInformation { get; set; }
        public virtual ICollection<CustomerTransactionItemProduct> CustomerTransactionItemProduct { get; set; }
        public virtual ICollection<ExpenseProductItem> ExpenseProductItem { get; set; }
        public virtual ICollection<QuoteItem> QuoteItem { get; set; }
    }
}
