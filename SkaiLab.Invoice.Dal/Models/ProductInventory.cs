using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ProductInventory
    {
        public ProductInventory()
        {
            ProductInventoryBalance = new HashSet<ProductInventoryBalance>();
            ProductInventoryHistory = new HashSet<ProductInventoryHistory>();
        }

        public long Id { get; set; }
        public long? DefaultLocationId { get; set; }

        public virtual Location DefaultLocation { get; set; }
        public virtual Product IdNavigation { get; set; }
        public virtual ICollection<ProductInventoryBalance> ProductInventoryBalance { get; set; }
        public virtual ICollection<ProductInventoryHistory> ProductInventoryHistory { get; set; }
    }
}
