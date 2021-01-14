using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Subscription
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double MontlyPrice { get; set; }
        public double MontlyRenewPrice { get; set; }
        public double YearlyPrice { get; set; }
        public double YearlyRenewPrice { get; set; }
        public int ProjectPlanId { get; set; }
        public double YearlySavePercent { get; set; }
    }
}
