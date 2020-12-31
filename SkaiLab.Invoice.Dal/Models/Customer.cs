using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Customer
    {
        public Customer()
        {
            CustomerTransaction = new HashSet<CustomerTransaction>();
            Quote = new HashSet<Quote>();
        }

        public long Id { get; set; }
        public string OrganisationId { get; set; }
        public string LegalName { get; set; }
        public string DisplayName { get; set; }
        public int CurrencyId { get; set; }
        public string TaxNumber { get; set; }
        public string ContactId { get; set; }
        public string BusinessRegistrationNumber { get; set; }
        public string LocalLegalName { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual ICollection<CustomerTransaction> CustomerTransaction { get; set; }
        public virtual ICollection<Quote> Quote { get; set; }
    }
}
