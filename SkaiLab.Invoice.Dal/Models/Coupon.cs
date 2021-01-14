using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Coupon
    {
        public Coupon()
        {
            UserPayment = new HashSet<UserPayment>();
        }

        public string Code { get; set; }
        public DateTime Created { get; set; }
        public double Rate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int NumberCanUse { get; set; }
        public int NumberOfUsed { get; set; }

        public virtual ICollection<UserPayment> UserPayment { get; set; }
    }
}
