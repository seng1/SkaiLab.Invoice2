using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class PayrollMonthTaxSalary
    {
        public PayrollMonthTaxSalary()
        {
            PayrollMonthTaxSalaryRange = new HashSet<PayrollMonthTaxSalaryRange>();
        }

        public long Id { get; set; }
        public decimal ChildOrSpouseAmount { get; set; }
        public decimal NoneResidentRate { get; set; }
        public decimal AdditionalBenefits { get; set; }

        public virtual PayrollMonth IdNavigation { get; set; }
        public virtual ICollection<PayrollMonthTaxSalaryRange> PayrollMonthTaxSalaryRange { get; set; }
    }
}
