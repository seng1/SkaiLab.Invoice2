using System;

namespace SkaiLab.Invoice.Models.Filter
{
    public class Filter
    {
        private DateTime? _fromDate { get; set; }
        private DateTime? _toDate { get; set; }
        public int Page { get; set; }
        public int TotalPage => TotalRow / PageSize;
        public int PageSize { get; set; }
        public string SearchText { get; set; }
        public string OrganisationId { get; set; }
        public int TotalRow { get; set; }
        public DateTypeFilter DateTypeFilter { get; set; }
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
}
