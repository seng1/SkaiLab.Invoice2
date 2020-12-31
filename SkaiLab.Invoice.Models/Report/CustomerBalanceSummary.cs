using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Report
{
    public class CustomerBalanceSummary
    {
        public Customer Customer { get; set; }
        public decimal Total { get; set; }
    }
}
