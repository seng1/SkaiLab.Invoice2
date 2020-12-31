using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class CustomerTransactionAttachment
    {
        public long Id { get; set; }
        public long CustomerTransactionId { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public bool IsFinalOfficalFile { get; set; }

        public virtual CustomerTransaction CustomerTransaction { get; set; }
    }
}
