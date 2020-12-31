using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class PayrollMonthTaxSalaryRange
    {
        public long Id { get; set; }
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public double TaxRate { get; set; }
        public long PayrollMonthTax { get; set; }

        public virtual PayrollMonthTaxSalary PayrollMonthTaxNavigation { get; set; }
    }
}
