using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class UserPaymentPayway
    {
        public long Id { get; set; }
        public string TransactionId { get; set; }

        public virtual UserPayment IdNavigation { get; set; }
    }
}
