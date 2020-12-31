using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Report
{
    public class ProfitAndLostSummary
    {
        public List<string> Headers { get; set; }
        public List<ProfitAndLostSummaryRowHeader> ProfitAndLostSummaryRowHeaders { get; set; }
        public ProfitAndLostSummaryTotal ProfitAndLostSummaryToal { get; set; }
    }
    public class ProfitAndLostSummaryRowHeader
    {
        public string Name { get; set; }
        public List<ProfitAndLostSummaryRow> ProfitAndLostSummaryRows { get; set; }
        public decimal Total { get; set; }
    }
    public class ProfitAndLostSummaryRow
    {
        public string Name { get; set; }
        public List<decimal> Values { get; set; }
    }
    public class ProfitAndLostSummaryTotal
    {
        public string Name { get; set; }
        public List<decimal> Values { get; set; }
    }
}
