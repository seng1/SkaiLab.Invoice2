using System;

namespace SkaiLab.Invoice.Models.Filter
{
    public class QuoteFilter:Filter
    {
        public long CustomerId { get; set; }
        public int StatusId { get; set; }
    }
}
