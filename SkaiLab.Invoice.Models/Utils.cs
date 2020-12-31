using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace SkaiLab.Invoice.Models
{
    public static class Utils
    {
        public static string FormatCurrency(decimal price,string currencySymble)
        {
            if(currencySymble== "៛")
            {
                return String.Format("{0:#,##0}", price)+ currencySymble;
            }
            return String.Format("{0:#,##0.00}", price)+ currencySymble;
        }

        public static DateTime? RessetFilterFromDate(DateTime? fromDate)
        {
            if(fromDate == null)
            {
                return null;
            }
            return new DateTime(fromDate.Value.Year, fromDate.Value.Month, fromDate.Value.Day, 0, 1, 0);
        }
        public static DateTime? RessetFilterToDate(DateTime? toDate)
        {
            if (toDate == null)
            {
                return null;
            }
            return new DateTime(toDate.Value.Year, toDate.Value.Month, toDate.Value.Day, 23, 23, 59);
        }
        public static DateTime? CurrentCambodiaTime()
        {
            var timeZone = TZConvert.GetTimeZoneInfo("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc), timeZone);
        }
        public static string ConvertInternationalNumberToKhmer(string num)
        {
            return num.Replace("0", "0").
                Replace("1", "១").
                Replace("2", "២").
                Replace("3", "៣").
                Replace("4", "៤").
                Replace("5", "៥").
                Replace("6", "៦").
                Replace("7", "៧").
                Replace("8", "៨").
                Replace("9", "៩")
                ;
        }
        public  static string GetKhmerMonthName(DateTime date)
        {
            string result = "មករា";
            switch (date.Day)
            {
                case 1:
                    result = "មករា";
                    break;
                case 2:
                    result = "កុម្ភៈ";
                    break;
                case 3:
                    result = "មីនា";
                    break;
                case 4:
                    result = "មេសា";
                    break;
                case 5:
                    result = "ឧសភា";
                    break;
                case 6:
                    result = "មិថុនា";
                    break;
                case 7:
                    result = "កក្កដា";
                    break;
                case 8:
                    result = "សីហា";
                    break;
                case 9:
                    result = "កញ្ញា";
                    break;
                case 10:
                    result = "តុលា";
                    break;
                case 11:
                    result = "វិច្ឆិកា";
                    break;
                case 12:
                    result = "ធ្នូ";
                    break;
            }
            return result;
        }
        public static string ReturnToError(IUrlHelper url, HttpContext context, string errorText)
        {
            var callbackUrl = url.Content("~/Error/Index");
            context.Session.SetString("ErrorText", errorText);
            return callbackUrl;
        }
    }
}
