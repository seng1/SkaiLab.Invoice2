using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ProductInventoryBalance
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long LocationId { get; set; }
        public int Quantity { get; set; }

        public virtual Location Location { get; set; }
        public virtual ProductInventory Product { get; set; }
    }
}
