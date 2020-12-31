using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class InventoryHistory
    {
        public long LocationId { get; set; }
        public Location Location { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class InventoryHistoryDetail : InventoryHistory
    {
        public DateTime Date { get; set; }
        public string RefNo { get; set; }
        public decimal Amount => Math.Abs(Quantity * UnitPrice);
    }
}
