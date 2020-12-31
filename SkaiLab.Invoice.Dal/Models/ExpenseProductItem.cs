using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ExpenseProductItem
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long? LocationId { get; set; }

        public virtual ExpenseItem IdNavigation { get; set; }
        public virtual Location Location { get; set; }
        public virtual Product Product { get; set; }
    }
}
