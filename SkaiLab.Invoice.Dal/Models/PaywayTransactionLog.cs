using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class PaywayTransactionLog
    {
        public string TransactionId { get; set; }
        public long UserPaymentId { get; set; }
        public DateTime Created { get; set; }

        public virtual UserPayment UserPayment { get; set; }
    }
}
