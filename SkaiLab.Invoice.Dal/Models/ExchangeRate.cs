using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ExchangeRate
    {
        public int FromOrganisationCurrencyId { get; set; }
        public int ToOrganisationCurrencyId { get; set; }
        public decimal ExchangeRate1 { get; set; }
        public bool IsAuto { get; set; }

        public virtual OrganisationCurrency FromOrganisationCurrency { get; set; }
        public virtual OrganisationCurrency ToOrganisationCurrency { get; set; }
    }
}
