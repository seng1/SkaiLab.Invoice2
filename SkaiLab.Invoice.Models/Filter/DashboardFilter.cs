using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Models.Filter
{
    public class DashboardFilter
    {
        public List<string> OrganisationIds { get; set; }
        private DashboardPeriodFilter _periodFilter;
        public DashboardPeriodFilter PeriodFilter
        {
            get
            {
                return _periodFilter;
            }
            set
            {
                _periodFilter = value;
                AssignDate();
            }
        }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        private void AssignDate()
        {
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now;
            DateTime date;
            switch (PeriodFilter.Id)
            {
                case (int)DashboardPeriodEnum.Last30Day:
                    fromDate = (DateTime)Utils.RessetFilterFromDate(Utils.CurrentCambodiaTime().Value.AddDays(-30));
                    toDate = Utils.RessetFilterToDate(Utils.CurrentCambodiaTime()).Value;
                    break;
                case (int)DashboardPeriodEnum.ThisMonth:
                    date = Utils.CurrentCambodiaTime().Value;
                    fromDate = new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
                    toDate = new DateTime(date.Year,date.Month, DateTime.DaysInMonth(date.Year, date.Month), 0, 0, 0, 0);
                    break;
                case (int)DashboardPeriodEnum.ThisQuarter:
                    fromDate = GetQuaterFromDate(Utils.CurrentCambodiaTime().Value);
                    date = fromDate.AddMonths(2);
                    toDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 1, 0, 0, 0);
                    break;
                case (int)DashboardPeriodEnum.ThisYear:
                    fromDate = new DateTime(Utils.CurrentCambodiaTime().Value.Year, 1, 1, 0, 0, 0, 0);
                    toDate = new DateTime(Utils.CurrentCambodiaTime().Value.Year, 12, DateTime.DaysInMonth(fromDate.Year, fromDate.Month), 0, 0, 0, 0);
                    break;
                case (int)DashboardPeriodEnum.LastMonth:
                    date = Utils.CurrentCambodiaTime().Value.AddMonths(-1);
                    fromDate = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                    toDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 0, 0, 0);
                    break;
                case (int)DashboardPeriodEnum.LastQuater:
                    fromDate = GetPreviouseQuaterFromDate(Utils.CurrentCambodiaTime().Value);
                    date = fromDate.AddMonths(2);
                    toDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 1, 0, 0, 0);
                    break;
                case (int)DashboardPeriodEnum.LastYear:
                    date = Utils.CurrentCambodiaTime().Value.AddYears(-1);
                    fromDate = new DateTime(date.Year, 1, 1, 0, 0, 0);
                    toDate = new DateTime(date.Year, 12, DateTime.DaysInMonth(date.Year, date.Month), 0, 0, 0);
                    break;
            }
            FromDate = Utils.RessetFilterFromDate(fromDate).Value;
            ToDate = Utils.RessetFilterToDate(toDate).Value;
        }
        private DateTime GetQuaterFromDate(DateTime date)
        {
            if (date.Month <= 3)
            {
                return new DateTime(date.Year, 1, 1, 1, 0, 0, 0);
            }
            if (date.Month <= 6)
            {
                return new DateTime(date.Year, 4, 1, 1, 0, 0, 0);
            }
            if (date.Month <= 9)
            {
                return new DateTime(date.Year, 7, 1, 1, 0, 0, 0);
            }
            return new DateTime(date.Year, 10, 1, 1, 0, 0, 0);
        }
        private DateTime GetPreviouseQuaterFromDate(DateTime date)
        {
            if (date.Month <= 3)
            {
                return new DateTime(date.Year-1, 10, 1, 1, 0, 0, 0);
            }
            if (date.Month <= 6)
            {
                return new DateTime(date.Year, 1, 1, 1, 0, 0, 0);
            }
            if (date.Month <= 9)
            {
                return new DateTime(date.Year, 4, 1, 1, 0, 0, 0);
            }
            return new DateTime(date.Year, 7, 1, 1, 0, 0, 0);
        }
    }
}
