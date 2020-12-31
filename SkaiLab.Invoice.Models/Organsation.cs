using System.ComponentModel.DataAnnotations;

namespace SkaiLab.Invoice.Models
{
    public class Organsation
    {
        public Organsation()
        {
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
        public string LegalLocalName { get; set; }
        public string ContactId { get; set; }
        public Contact Contact { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public Currency TaxCurrency { get; set; }
        public bool DeclareTax { get; set; }
        public OrganisationBaseCurrency OrganisationBaseCurrency { get; set; }
        public Currency BaseCurrency { get; set; }

    }
    public class OrganisationBaseCurrency
    {
        public int BaseCurrencyId { get; set; }
        public decimal TaxExchangeRate { get; set; }
        public int TaxCurrencyId { get; set; }
    }
}
