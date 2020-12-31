using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ProductInventoryHistoryIn
    {
        public ProductInventoryHistoryIn()
        {
            ProductInventoryHistoryOut = new HashSet<ProductInventoryHistoryOut>();
        }

        public long Id { get; set; }

        public virtual ProductInventoryHistory IdNavigation { get; set; }
        public virtual ICollection<ProductInventoryHistoryOut> ProductInventoryHistoryOut { get; set; }
    }
}
