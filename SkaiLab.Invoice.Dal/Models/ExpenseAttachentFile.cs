using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ExpenseAttachentFile
    {
        public long Id { get; set; }
        public long ExpenseId { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public bool? IsFinalOfficalFile { get; set; }

        public virtual Expense Expense { get; set; }
    }
}
