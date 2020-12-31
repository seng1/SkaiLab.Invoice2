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
        public double Price { get; set; }
        public double? YearDiscountRate { get; set; }
        public int ProjectPlanId { get; set; }
    }
}
