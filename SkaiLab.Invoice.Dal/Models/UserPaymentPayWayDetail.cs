using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class UserPaymentPayWayDetail
    {
        public long Id { get; set; }
        public double Amount { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }
        public string Apv { get; set; }
        public string PaymentType { get; set; }
        public string SourceOfFund { get; set; }
        public string PaidBy { get; set; }

        public virtual UserPayment IdNavigation { get; set; }
    }
}
