using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class PayrollEmployeeNoneTax
    {
        public long Id { get; set; }
        public decimal Salary { get; set; }
        public decimal? OtherBenefit { get; set; }

        public virtual PayrollEmployee IdNavigation { get; set; }
    }
}
