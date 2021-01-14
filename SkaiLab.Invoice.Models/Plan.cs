using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double MonthlyPrice { get; set; }
        public double MonthlyRenewalPrice { get; set; }
        public double YearlyPrice { get; set; }
        public double YearlyRenewalPrice { get; set; }
    }
}
