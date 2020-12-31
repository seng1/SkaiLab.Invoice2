using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class OrganisationBaseCurrency
    {
        public string OrganisationId { get; set; }
        public int TaxCurrencyId { get; set; }
        public int BaseCurrencyId { get; set; }

        public virtual Currency BaseCurrency { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual Currency TaxCurrency { get; set; }
    }
}
