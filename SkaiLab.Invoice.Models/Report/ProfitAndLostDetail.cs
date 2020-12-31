using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Report
{
    public class ProfitAndLostDetail
    {
        public ProfitAndLostDetailTotal ProfitAndLostDetailTotal { get; set; }
        public List<ProfitAndLostDetailParent> ProfitAndLostDetailParents { get; set; }

    }
    public class ProfitAndLostDetailTotal
    {
        public string Name { get; set; }
        public decimal Total { get; set; }
    }
    public class ProfitAndLostDetailParent
    {
        public string Name { get; set; }
        public decimal Total { get; set; }
        public bool IsExpand { get; set; }
        public List<ProfitAndLostDetailItem> ProfitAndLostDetailItems { get; set; }
    }
    public class ProfitAndLostDetailItem
    {
        public DateTime Date { get; set; }
        public string TransactionType { get; set; }
        public string Number { get; set; }
        public string ClientOrVendorName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public long ParentId { get; set; }
    }
}
