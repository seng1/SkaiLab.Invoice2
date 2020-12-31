using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class Receipt
    {
        public Organsation Orgainsation { get; set; }
        public Customer Customer { get; set; }
        public string Purpose { get; set; }
        public decimal Total { get; set; }
        public string AmountInWord { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsTaxInvoice { get; set; }
        public DateTime Date { get; set; }
    }
}
