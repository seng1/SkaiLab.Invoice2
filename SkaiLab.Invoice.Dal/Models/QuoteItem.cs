using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class QuoteItem
    {
        public long Id { get; set; }
        public long QuoteId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public double? DiscountRate { get; set; }
        public long? TaxId { get; set; }
        public decimal LineTotal { get; set; }
        public decimal LineTotalIncludeTax { get; set; }
        public string Description { get; set; }
        public long ProductId { get; set; }
        public long? LocationId { get; set; }

        public virtual Location Location { get; set; }
        public virtual Product Product { get; set; }
        public virtual Quote Quote { get; set; }
        public virtual Tax Tax { get; set; }
    }
}
