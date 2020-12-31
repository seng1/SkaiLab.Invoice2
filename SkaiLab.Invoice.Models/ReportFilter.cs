using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class ReportFilter
    {

        private DateTime? _fromDate { get; set; }
        private DateTime? _toDate { get; set; }
        public List<string> OrganisationIds { get; set; }
        public DateTime? FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = Utils.RessetFilterFromDate(value);
            }
        }
        public DateTime? ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = Utils.RessetFilterToDate(value);
            }
        }


    }
    public class InventoryHistoryFilter: ReportFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

    }
    public class ProfitAndLostSummaryFilter:ReportFilter
    {
        public DisplayColumn DisplayColumn { get; set; }
    }
    public class DisplayColumn
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
