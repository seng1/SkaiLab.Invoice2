using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class UserPaymentInvoice
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public int Year { get; set; }
        public string Number { get; set; }

        public virtual UserPayment IdNavigation { get; set; }
    }
}
