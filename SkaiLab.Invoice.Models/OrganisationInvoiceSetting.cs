using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class OrganisationInvoiceSetting
    {
        public string Id { get; set; }
        public string TermAndConditionForQuote { get; set; }
        public string TermAndConditionForInvoice { get; set; }
        public string TermAndConditionForPurchaseOrder { get; set; }
    }
}
