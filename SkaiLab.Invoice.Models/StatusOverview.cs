using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class StatusOverview
    {
        public int StatusId { get; set; }
        public string CurrencySymbole { get; set; }
        public string StatusName { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }

    }
}
