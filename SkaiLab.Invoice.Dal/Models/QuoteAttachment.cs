using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class QuoteAttachment
    {
        public long Id { get; set; }
        public long QuoteId { get; set; }
        public string FileUrl { get; set; }
        public bool IsFinalOfficalFile { get; set; }
        public string FileName { get; set; }

        public virtual Quote Quote { get; set; }
    }
}
