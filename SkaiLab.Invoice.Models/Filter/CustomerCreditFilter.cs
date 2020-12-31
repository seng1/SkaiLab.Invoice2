using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Filter
{
    public class CustomerCreditFilter: Filter
    {
        public long CustomerId { get; set; }
        public int StatusId { get; set; }
    }
}
