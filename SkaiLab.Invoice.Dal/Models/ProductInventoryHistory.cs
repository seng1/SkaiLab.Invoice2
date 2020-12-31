using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ProductInventoryHistory
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long LocationId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime Created { get; set; }
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
        public string RefNo { get; set; }

        public virtual Location Location { get; set; }
        public virtual ProductInventory Product { get; set; }
        public virtual ProductInventoryHistoryIn ProductInventoryHistoryIn { get; set; }
        public virtual ProductInventoryHistoryOut ProductInventoryHistoryOut { get; set; }
    }
}
