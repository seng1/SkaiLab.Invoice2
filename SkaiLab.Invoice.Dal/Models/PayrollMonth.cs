using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class PayrollMonth
    {
        public PayrollMonth()
        {
            PayrollEmployee = new HashSet<PayrollEmployee>();
        }

        public long Id { get; set; }
        public string Month { get; set; }
        public decimal Total { get; set; }
        public double ExchangeRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual PayrollMonthTaxSalary PayrollMonthTaxSalary { get; set; }
        public virtual ICollection<PayrollEmployee> PayrollEmployee { get; set; }
    }
}
