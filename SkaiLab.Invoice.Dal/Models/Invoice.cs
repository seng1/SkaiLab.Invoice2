using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Invoice
    {
        public long Id { get; set; }
        public int StatusId { get; set; }

        public virtual CustomerTransaction IdNavigation { get; set; }
        public virtual InvoiceStatus Status { get; set; }
        public virtual InvoiceQuote InvoiceQuote { get; set; }
    }
}
