using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Location
    {
        public Location()
        {
            CustomerTransactionItemProduct = new HashSet<CustomerTransactionItemProduct>();
            ExpenseProductItem = new HashSet<ExpenseProductItem>();
            ProductInventory = new HashSet<ProductInventory>();
            ProductInventoryBalance = new HashSet<ProductInventoryBalance>();
            ProductInventoryHistory = new HashSet<ProductInventoryHistory>();
            QuoteItem = new HashSet<QuoteItem>();
        }

        public long Id { get; set; }
        public string OrganisationId { get; set; }
        public string Name { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual ICollection<CustomerTransactionItemProduct> CustomerTransactionItemProduct { get; set; }
        public virtual ICollection<ExpenseProductItem> ExpenseProductItem { get; set; }
        public virtual ICollection<ProductInventory> ProductInventory { get; set; }
        public virtual ICollection<ProductInventoryBalance> ProductInventoryBalance { get; set; }
        public virtual ICollection<ProductInventoryHistory> ProductInventoryHistory { get; set; }
        public virtual ICollection<QuoteItem> QuoteItem { get; set; }
    }
}
