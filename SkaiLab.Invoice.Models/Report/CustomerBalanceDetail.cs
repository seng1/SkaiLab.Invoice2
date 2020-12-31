using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Models.Report
{
    public class CustomerBalanceDetail
    {
        public List<CustomerBalanceDetailHeader> CustomerBalanceDetailHeaders { get; set; }
        public decimal Total
        {
            get
            {
                if (CustomerBalanceDetailHeaders == null)
                {
                    return 0;
                }
                return CustomerBalanceDetailHeaders.Sum(u => u.Total);
            }
        }
    }
    public class CustomerBalanceDetailHeader
    {
        public Customer Customer { get; set; }
        public List<CustomerBalanceDetailItem> CustomerBalanceDetailItems { get; set; }
        public decimal Total
        {
            get
            {
                if (CustomerBalanceDetailItems == null)
                {
                    return 0;
                }
                return CustomerBalanceDetailItems.Sum(u => u.Amount);
            }
        }
        public bool IsExpand { get; set; }
    }
    public class CustomerBalanceDetailItem
    {
        public DateTime Date { get; set; }
        public string TransactionType { get; set; }
        public string Number { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal Amount { get; set; }
        public Customer Customer { get; set; }
        public long ParentId { get; set; }
        public DateTime? PaidDate { get; set; }
        public bool IsOverDue
        {
            get
            {
                if (PaidDate == null)
                {
                    return false;
                }
                if (DueDate == null)
                {
                    return false;
                }
                return DueDate <= Utils.CurrentCambodiaTime();
            }
        }
    }
}
