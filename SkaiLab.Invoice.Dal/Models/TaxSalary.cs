using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class TaxSalary
    {
        public int Id { get; set; }
        public decimal ChildOrSpouseAmount { get; set; }
        public decimal NoneResidentRate { get; set; }
        public decimal AdditionalBenefits { get; set; }
    }
}
