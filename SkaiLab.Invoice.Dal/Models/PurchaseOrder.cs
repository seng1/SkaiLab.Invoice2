using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class PurchaseOrder
    {
        public long Id { get; set; }

        public virtual Expense IdNavigation { get; set; }
    }
}
