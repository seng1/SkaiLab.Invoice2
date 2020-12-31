using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class QuoteStatus
    {
        public QuoteStatus()
        {
            Quote = new HashSet<Quote>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameKh { get; set; }

        public virtual ICollection<Quote> Quote { get; set; }
    }
}
