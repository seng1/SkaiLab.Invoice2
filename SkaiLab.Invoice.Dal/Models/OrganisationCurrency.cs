using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class OrganisationCurrency
    {
        public int Id { get; set; }
        public string OrganisationId { get; set; }
        public int CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
