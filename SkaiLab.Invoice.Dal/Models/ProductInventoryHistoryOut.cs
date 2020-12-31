using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ProductInventoryHistoryOut
    {
        public long Id { get; set; }
        public long InventoryInId { get; set; }

        public virtual ProductInventoryHistory IdNavigation { get; set; }
        public virtual ProductInventoryHistoryIn InventoryIn { get; set; }
    }
}
