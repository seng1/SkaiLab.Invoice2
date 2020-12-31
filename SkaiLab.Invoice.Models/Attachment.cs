using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class Attachment
    {
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public bool IsFinalOfficalFile { get; set; }
        public string RefNo { get; set; }
        public long ExpenseId { get; set; }
    }
}
