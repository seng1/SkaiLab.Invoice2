using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class PayrollEmployeeTax
    {
        public long Id { get; set; }
        public decimal Salary { get; set; }
        public decimal SalaryTax { get; set; }
        public decimal? OtherBenefit { get; set; }
        public decimal? OtherBenefitTaxDeduct { get; set; }
        public bool IsResidentEmployee { get; set; }
        public bool ConfederationThatHosts { get; set; }
        public int NumberOfChilds { get; set; }

        public virtual PayrollEmployee IdNavigation { get; set; }
    }
}
