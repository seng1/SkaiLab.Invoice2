using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameKh { get; set; }
        public double Price { get; set; }
        public double? YearDiscount { get; set; }
        public int ProjectPlanId { get; set; }

        public virtual ProjectPlan ProjectPlan { get; set; }
    }
}
