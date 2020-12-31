using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class OrganisationInvoiceSetting
    {
        public string Id { get; set; }
        public string TermAndConditionForQuote { get; set; }
        public string TermAndConditionForInvoice { get; set; }
        public string TermAndConditionForPurchaseOrder { get; set; }

        public virtual Organisation IdNavigation { get; set; }
    }
}
