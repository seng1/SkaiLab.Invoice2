using System.Collections.Generic;
namespace SkaiLab.Invoice.Models
{
    public class NewCurrency
    {
        public List<Currency> Currencies { get; set; }
        public Currency BaseCurrency { get; set; }
        public Currency TaxCurrency { get; set; }
    }
}
