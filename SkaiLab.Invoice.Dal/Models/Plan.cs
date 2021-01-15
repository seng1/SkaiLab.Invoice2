using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Plan
    {
        public Plan()
        {
            UserPayment = new HashSet<UserPayment>();
            UserPlan = new HashSet<UserPlan>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameKh { get; set; }
        public double MonthlyPrice { get; set; }
        public int ProjectPlanId { get; set; }
        public double YearlyPrice { get; set; }
        public double MonthlyRenewPrice { get; set; }
        public double YearlyRenewPrice { get; set; }
        public double YearlySavePercent { get; set; }

        public virtual ProjectPlan ProjectPlan { get; set; }
        public virtual ICollection<UserPayment> UserPayment { get; set; }
        public virtual ICollection<UserPlan> UserPlan { get; set; }
    }
}
