
using System.Collections.Generic;

namespace SkaiLab.Invoice.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbole { get; set; }
        public List<string> Notes { get; set; }
        public List<string> ExchangeRateTexts { get; set; }
        public List<CurrencyExchangeRate> ExchangeRates { get; set; }

    }
    public class CurrencyExchangeRate
    {
        public int CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public bool IsAuto { get; set; }
        public Currency Currency { get; set; }
    }
}
