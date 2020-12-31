using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class PayrollEmployee
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public long PayrollMonthId { get; set; }
        public DateTime Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Total { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual PayrollMonth PayrollMonth { get; set; }
        public virtual PayrollEmployeeNoneTax PayrollEmployeeNoneTax { get; set; }
        public virtual PayrollEmployeeTax PayrollEmployeeTax { get; set; }
    }
}
