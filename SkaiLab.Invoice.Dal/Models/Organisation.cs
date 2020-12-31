using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Organisation
    {
        public Organisation()
        {
            Customer = new HashSet<Customer>();
            CustomerTransaction = new HashSet<CustomerTransaction>();
            Employee = new HashSet<Employee>();
            Expense = new HashSet<Expense>();
            Location = new HashSet<Location>();
            OrganisationCurrency = new HashSet<OrganisationCurrency>();
            OrganisationInvitingUser = new HashSet<OrganisationInvitingUser>();
            OrganisationInvitingUserMenuFeature = new HashSet<OrganisationInvitingUserMenuFeature>();
            OrganisationUser = new HashSet<OrganisationUser>();
            OrganisationUserMenuFeature = new HashSet<OrganisationUserMenuFeature>();
            PayrollMonth = new HashSet<PayrollMonth>();
            Product = new HashSet<Product>();
            Quote = new HashSet<Quote>();
            Tax = new HashSet<Tax>();
            Vendor = new HashSet<Vendor>();
            WorkingOrganisation = new HashSet<WorkingOrganisation>();
        }

        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string LegalName { get; set; }
        public string LogoUrl { get; set; }
        public int? OrganisationTypeId { get; set; }
        public string LineBusiness { get; set; }
        public string BussinessRegistrationNumber { get; set; }
        public string Description { get; set; }
        public string TaxNumber { get; set; }
        public string ContactId { get; set; }
        public string LegalLocalName { get; set; }
        public bool DeclareTax { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual OrganisationType OrganisationType { get; set; }
        public virtual OrganisationBaseCurrency OrganisationBaseCurrency { get; set; }
        public virtual OrganisationInvoiceSetting OrganisationInvoiceSetting { get; set; }
        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<CustomerTransaction> CustomerTransaction { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<Expense> Expense { get; set; }
        public virtual ICollection<Location> Location { get; set; }
        public virtual ICollection<OrganisationCurrency> OrganisationCurrency { get; set; }
        public virtual ICollection<OrganisationInvitingUser> OrganisationInvitingUser { get; set; }
        public virtual ICollection<OrganisationInvitingUserMenuFeature> OrganisationInvitingUserMenuFeature { get; set; }
        public virtual ICollection<OrganisationUser> OrganisationUser { get; set; }
        public virtual ICollection<OrganisationUserMenuFeature> OrganisationUserMenuFeature { get; set; }
        public virtual ICollection<PayrollMonth> PayrollMonth { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<Quote> Quote { get; set; }
        public virtual ICollection<Tax> Tax { get; set; }
        public virtual ICollection<Vendor> Vendor { get; set; }
        public virtual ICollection<WorkingOrganisation> WorkingOrganisation { get; set; }
    }
}
