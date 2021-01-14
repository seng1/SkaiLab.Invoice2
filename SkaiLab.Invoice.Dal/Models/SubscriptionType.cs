using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class SubscriptionType
    {
        public SubscriptionType()
        {
            UserPayment = new HashSet<UserPayment>();
            UserPlan = new HashSet<UserPlan>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserPayment> UserPayment { get; set; }
        public virtual ICollection<UserPlan> UserPlan { get; set; }
    }
}
