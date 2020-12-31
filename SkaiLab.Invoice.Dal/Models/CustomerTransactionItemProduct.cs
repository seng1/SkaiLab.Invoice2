using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class CustomerTransactionItemProduct
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long? LocationId { get; set; }

        public virtual CustomerTransactionItem IdNavigation { get; set; }
        public virtual Location Location { get; set; }
        public virtual Product Product { get; set; }
    }
}
