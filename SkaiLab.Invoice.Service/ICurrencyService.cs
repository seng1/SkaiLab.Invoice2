using SkaiLab.Invoice.Models;
using System.Collections.Generic;
namespace SkaiLab.Invoice.Service
{
    public interface ICurrencyService : IService
    {
        List<Currency> GetCurrencies(string organisationId);
        NewCurrency GetCurrenciesForCreate(string organisationId);
        void CreateCurrency(string organisationId, Currency currency);
        Currency GetCurrencyWithExchangeRate(int curencyId, string organisationId);
        void UpdateExchangeRate(string organisationId, Currency currency);
        List<Currency> GetCurrenciesWithoutNote(string organisationId);
        List<Currency> GetCurrenciesWithExchangeRate(string organisationId);
        List<Currency> GetCurrencies();
    }
}
