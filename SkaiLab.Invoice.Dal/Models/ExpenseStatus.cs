using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ExpenseStatus
    {
        public ExpenseStatus()
        {
            Expense = new HashSet<Expense>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameKh { get; set; }

        public virtual ICollection<Expense> Expense { get; set; }
    }
}
