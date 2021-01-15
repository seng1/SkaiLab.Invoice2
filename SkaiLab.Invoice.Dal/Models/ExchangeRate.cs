using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ExchangeRate
    {
        public int FromCurrencyId { get; set; }
        public int ToCurrencyId { get; set; }
        public string OrganisationId { get; set; }
        public decimal ExchangeRate1 { get; set; }
        public bool IsAuto { get; set; }

        public virtual Currency FromCurrency { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual Currency ToCurrency { get; set; }
    }
}
