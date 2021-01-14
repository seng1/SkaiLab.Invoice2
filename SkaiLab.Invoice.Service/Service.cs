using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkaiLab.Invoice.Dal.Models;
using SkaiLab.Invoice.Models;
using TimeZoneConverter;
namespace SkaiLab.Invoice.Service
{
    public class Service : IService
    {
        private readonly IDataContext dataContext;
        public Service(IDataContext context)
        {
            this.dataContext = context;
        }
        protected InvoiceContext Context()
        {
            return dataContext.Context();
        }
        public  string FormatCurrency(decimal price)
        {
            return String.Format("{0:#,##0.00}", price);
        }
        public Models.Currency GetOrganisationBaseCurrency(string organisationId)
        {
            using var context = Context();

            var currency = context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId);
            return new Models.Currency
            {
                Code = currency.BaseCurrency.Code,
                Id = currency.BaseCurrencyId,
                Name = currency.BaseCurrency.Name,
                Symbole = currency.BaseCurrency.Symbole
            };
        }
        public int GetOrganisationBaseCurrencyId(string organisationId)
        {
            return GetOrganisationBaseCurrency(organisationId).Id;
        }
        public int GetOrganisationBaseCurrencyId(InvoiceContext context, string organisationId)
        {
            return context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId).BaseCurrencyId;
        }
        public string UploadFile(string baseString)
        {
            var context = dataContext.BlobContainerClient();
            var extension = baseString.Split(';')[0].Split('/')[1];
            var fileName = Guid.NewGuid() + "." + extension;
            baseString = baseString.Split(',')[1];
            var bytes = Convert.FromBase64String(baseString);
            var blobClient = context.GetBlobClient(fileName);
            blobClient.Upload(new MemoryStream(bytes));
            return Option.BlobStorage.ContainerUrl + "/" + fileName;
        }
        public string UploadFile(string baseString,string fileName=null)
        {
            var context = dataContext.BlobContainerClient();
            var extension = baseString.Split(';')[0].Split('/')[1];
            if (string.IsNullOrEmpty(fileName))
            {
                 fileName = Guid.NewGuid() + "." + extension;
            }
            else
            {
                fileName = fileName.Split('.')[0] + "-" + Guid.NewGuid().ToString() +"."+ extension;
            }
            baseString = baseString.Split(',')[1];
            var bytes = Convert.FromBase64String(baseString);
            var blobClient = context.GetBlobClient(fileName);
            blobClient.Upload(new MemoryStream(bytes));
            return Option.BlobStorage.ContainerUrl + "/" + fileName;
        }
        public Models.Currency GetTaxCurrency(InvoiceContext context, string organisationId)
        {
            var currency = context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId);
            return new Models.Currency
            {
                Code = currency.TaxCurrency.Code,
                Id = currency.TaxCurrencyId,
                Name = currency.TaxCurrency.Name,
                Symbole = currency.TaxCurrency.Symbole
            };
        }
        public Models.Currency GetTaxCurrency(string organisationId)
        {
            using var context = Context();
            return GetTaxCurrency(context, organisationId);
        }

        public Option Option => dataContext.Option;
        protected DateTime CurrentCambodiaTime
        {
            get
            {
                return Utils.CurrentCambodiaTime().Value;
            }
        }
        protected List<Models.Currency> GetCurrenciesWithExchangeRate(InvoiceContext context, string organisationId)
        {
            var currencies = context.OrganisationCurrency.Where(u => u.OrganisationId == organisationId)
                .Select(u => new Models.Currency
                {
                    Code = u.Currency.Code,
                    Id = u.CurrencyId,
                    Name = u.Currency.Name,
                    Symbole = u.Currency.Symbole,
                    ExchangeRates = u.ExchangeRateFromOrganisationCurrency.Select(u => new CurrencyExchangeRate
                    {
                        CurrencyId = u.ToOrganisationCurrencyId,
                        Currency = new Models.Currency
                        {
                            Code = u.ToOrganisationCurrency.Currency.Code,
                            Id = u.ToOrganisationCurrency.CurrencyId,
                            Name = u.ToOrganisationCurrency.Currency.Name,
                            Symbole = u.ToOrganisationCurrency.Currency.Symbole
                        },
                        ExchangeRate = u.ExchangeRate1
                    }).ToList()
                }).ToList();
            return currencies;
        }
        public List<Models.Tax> GetTaxesIncludeComponent(InvoiceContext context, string organisationId)
        {
            var taxes = context.Tax.Where(u => u.OrganisationId == organisationId)
                .Select(u => new Models.Tax
                {
                    Id = u.Id,
                    Name = u.Name,
                    Components = u.TaxComponent.Select(t => new Models.TaxComponent
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Rate = t.Rate
                    }).ToList()
                }).ToList();
            return taxes;
        }

        public bool CheckPermission(string userId, string organisationId, int menuFeatureId)
        {
            using var context = Context();
            return context.OrganisationUserMenuFeature.FirstOrDefault(u => u.UserId == userId && u.OrganisationId == organisationId && u.MenuFeatureId == menuFeatureId) != null;
        }

        public bool CheckPermission(string userId, string organisationId, int[] menuFeatureIds)
        {
            using var context = Context();
            return context.OrganisationUserMenuFeature.Any(u => u.UserId == userId && u.OrganisationId == organisationId && menuFeatureIds.Any(t=>t==u.MenuFeatureId));
        }

        public bool IsValidLicense(string organisationId)
        {
            using var context = Context();
            var organisationUser = context.OrganisationUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.IsOwner);
            if (organisationUser.User.UserPlan.Expire == null || organisationUser.User.UserPlan.Expire<CurrentCambodiaTime)
            {
                return false;
            }
            return true;
        }

        public string UserId => dataContext.UserId;
        public string OrganisationId => dataContext.OrganisationId;

        public string Language => dataContext.Language;

        public IAppResource AppResource => dataContext.AppResource;

        public bool IsKhmer => Language=="km-KH";
    }
}
