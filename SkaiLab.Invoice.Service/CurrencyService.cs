using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class CurrencyService:Service,ICurrencyService
    {
        public CurrencyService(IDataContext context) : base(context)
        {

        }

        public void CreateCurrency(string organisationId, Currency currency)
        {
            using var context = Context();
            if (context.OrganisationCurrency.Any(u => u.OrganisationId == organisationId && u.CurrencyId == currency.Id))
            {
                throw new Exception("Currency already created");
            }
            if (currency.ExchangeRates == null || !currency.ExchangeRates.Any() || currency.ExchangeRates.Any(u => u.ExchangeRate <= 0))
            {
                throw new Exception("Exchange rate is require");
            }
            var organisationCurrency = new Dal.Models.OrganisationCurrency
            {
                CurrencyId = currency.Id,
                OrganisationId = organisationId
            };

            context.OrganisationCurrency.Add(organisationCurrency);
            foreach (var exchangeRate in currency.ExchangeRates)
            {
                context.ExchangeRate.Add(new Dal.Models.ExchangeRate
                {
                    ExchangeRate1 = (decimal)exchangeRate.ExchangeRate,
                    FromCurrencyId = organisationCurrency.CurrencyId,
                    IsAuto = exchangeRate.IsAuto,
                    ToCurrencyId = context.OrganisationCurrency.FirstOrDefault(u => u.OrganisationId == organisationId && u.CurrencyId == exchangeRate.CurrencyId).CurrencyId,
                    OrganisationId=organisationId
                });
            }
            context.SaveChanges();
        }

        public List<Currency> GetCurrencies(string organisationId)
        {
            using var context = Context();
            var exchangeRates = context.ExchangeRate.Where(u => u.OrganisationId == organisationId)
                .Select(u=>new
                {
                    u.FromCurrencyId,
                    CurrencyCode=u.FromCurrency.Code,
                    u.ExchangeRate1,
                    ToCurrencyCode=u.ToCurrency.Code
                }).ToList()
                .Select(u => new
                {
                    u.FromCurrencyId,
                    ExchangeRateText =$"1 {u.CurrencyCode} = {FormatCurrency(u.ExchangeRate1)} {u.ToCurrencyCode}"
                }).ToList();
            var currencies = context.OrganisationCurrency.Where(u=>u.OrganisationId==organisationId).ToList().Select(u => new Currency
            {
                Code=u.Currency.Code,
                Id=u.Currency.Id,
                Name=u.Currency.Name,
                Symbole=u.Currency.Symbole,
                ExchangeRateTexts= exchangeRates.Where(t=>t.FromCurrencyId==u.Id).Select(u=>u.ExchangeRateText).ToList(),
                Notes=new List<string>()
            }).ToList();
            var baseCurrency = context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId);

            foreach(var currency in currencies)
            {
                if (currency.Id == baseCurrency.TaxCurrencyId)
                {
                    currency.Notes.Add("Tax Currency");
                }
                if (currency.Id == baseCurrency.BaseCurrencyId)
                {
                    currency.Notes.Add("Company Base Currency");
                }
            }
            return currencies;

        }

        public List<Currency> GetCurrencies()
        {
            using var context = Context();
            var currencies = context.Currency.Select(u => new Currency
            {
                Code=u.Code,
                Id=u.Id,
                Name=u.Name,
                Symbole=u.Symbole
            }).ToList();
            return currencies.ToList();
        }

        public NewCurrency GetCurrenciesForCreate(string organisationId)
        {
            using var context = Context();
            var currencies = from c in context.Currency
                             where !c.OrganisationCurrency.Any(u => u.OrganisationId == organisationId)
                             select new Currency
                             {
                                 Code = c.Code,
                                 Id = c.Id,
                                 Name = c.Name,
                                 Symbole = c.Symbole
                             };
            var organisation = context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId);
            return new NewCurrency
            {
                Currencies = currencies.ToList(),
                BaseCurrency=new Currency
                {
                    Id=organisation.BaseCurrency.Id,
                    Code=organisation.BaseCurrency.Code,
                    Name=organisation.BaseCurrency.Name,
                    Symbole=organisation.BaseCurrency.Symbole
                },
                TaxCurrency=new Currency
                {
                     Symbole=organisation.TaxCurrency.Symbole,
                     Name=organisation.TaxCurrency.Name,
                     Code=organisation.TaxCurrency.Code,
                     Id=organisation.TaxCurrency.Id
                }
            };
        }

        public List<Currency> GetCurrenciesWithExchangeRate(string organisationId)
        {
            using var context = Context();
            return GetCurrenciesWithExchangeRate(context, organisationId);
        }

        public List<Currency> GetCurrenciesWithoutNote(string organisationId)
        {
            using var context = Context();
            var currencies = context.OrganisationCurrency.Where(u => u.OrganisationId == organisationId).Select(u => new Currency
            {
                Code = u.Currency.Code,
                Id = u.Currency.Id,
                Name = u.Currency.Name,
                Symbole = u.Currency.Symbole,
            }).ToList();
            return currencies;
        }

        public Currency GetCurrencyWithExchangeRate(int curencyId, string organisationId)
        {
            using var context = Context();
            var currency = context.OrganisationCurrency.FirstOrDefault(u => u.CurrencyId == curencyId && u.OrganisationId == organisationId);
            if (currency == null)
            {
                throw new Exception("Currency not found");
            }
            var exchangeRates = context.ExchangeRate.Where(u => u.FromCurrencyId == currency.CurrencyId && u.OrganisationId==organisationId);
            if (!exchangeRates.Any())
            {
                throw new Exception("Currency exchange rate doesn't exist");
            }
            return new Currency
            {
                Id = currency.CurrencyId,
                Code = currency.Currency.Code,
                Name=currency.Currency.Name,
                Symbole=currency.Currency.Symbole,
                ExchangeRates=exchangeRates.Select(u=>new CurrencyExchangeRate
                {
                    CurrencyId=u.ToCurrencyId,
                    ExchangeRate=u.ExchangeRate1,
                    IsAuto=u.IsAuto,
                    Currency=new Currency
                    {
                        Code=u.ToCurrency.Code,
                        Id=u.ToCurrencyId,
                        Name=u.ToCurrency.Name,
                        Symbole=u.ToCurrency.Symbole
                    }
                }).ToList()
            };
        }

        public void UpdateExchangeRate(string organisationId, Currency currency)
        {
            using var context = Context();
            var currencyDb = context.OrganisationCurrency.FirstOrDefault(u => u.CurrencyId == currency.Id && u.OrganisationId == organisationId);
            if (currencyDb == null)
            {
                throw new Exception("No Currency to update");
            }
            var exchangeRates = context.ExchangeRate.Where(u => u.FromCurrencyId == currency.Id);
            if (!exchangeRates.Any())
            {
                throw new Exception("No currency exchange rate doesn't exist");
            }
            foreach(var exchangeRate in exchangeRates.ToList())
            {
                var ex = currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == exchangeRate.ToCurrencyId);
                exchangeRate.ExchangeRate1 = ex.ExchangeRate;
                exchangeRate.IsAuto = ex.IsAuto;
            }
            context.SaveChanges();
        }
    }
}
