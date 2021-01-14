using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class UserPlan
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public int PlanId { get; set; }
        public DateTime? Expire { get; set; }
        public int SubcriptionId { get; set; }
        public double Price { get; set; }
        public double RenewPrice { get; set; }
        public bool IsTrail { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual ProjectPlan Project { get; set; }
        public virtual SubscriptionType Subcription { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
