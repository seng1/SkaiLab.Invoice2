using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class InvoiceQuote
    {
        public long Id { get; set; }
        public long QuoteId { get; set; }

        public virtual Invoice IdNavigation { get; set; }
        public virtual Quote Quote { get; set; }
    }
}
