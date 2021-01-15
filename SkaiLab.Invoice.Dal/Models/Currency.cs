using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Currency
    {
        public Currency()
        {
            Customer = new HashSet<Customer>();
            CustomerTransaction = new HashSet<CustomerTransaction>();
            ExchangeRateFromCurrency = new HashSet<ExchangeRate>();
            ExchangeRateToCurrency = new HashSet<ExchangeRate>();
            Expense = new HashSet<Expense>();
            OrganisationBaseCurrencyBaseCurrency = new HashSet<OrganisationBaseCurrency>();
            OrganisationBaseCurrencyTaxCurrency = new HashSet<OrganisationBaseCurrency>();
            OrganisationCurrency = new HashSet<OrganisationCurrency>();
            Quote = new HashSet<Quote>();
            Vendor = new HashSet<Vendor>();
        }

        public string Code { get; set; }
        public string Symbole { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<CustomerTransaction> CustomerTransaction { get; set; }
        public virtual ICollection<ExchangeRate> ExchangeRateFromCurrency { get; set; }
        public virtual ICollection<ExchangeRate> ExchangeRateToCurrency { get; set; }
        public virtual ICollection<Expense> Expense { get; set; }
        public virtual ICollection<OrganisationBaseCurrency> OrganisationBaseCurrencyBaseCurrency { get; set; }
        public virtual ICollection<OrganisationBaseCurrency> OrganisationBaseCurrencyTaxCurrency { get; set; }
        public virtual ICollection<OrganisationCurrency> OrganisationCurrency { get; set; }
        public virtual ICollection<Quote> Quote { get; set; }
        public virtual ICollection<Vendor> Vendor { get; set; }
    }
}
