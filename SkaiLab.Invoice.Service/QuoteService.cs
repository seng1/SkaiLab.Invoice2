using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class QuoteService : Service, IQuoteService
    {
        private readonly ITaxService taxService;
        private readonly ILocationService locationService;
        private readonly ICurrencyService currencyService;
        public QuoteService(IDataContext context, ITaxService taxService, ILocationService locationService, ICurrencyService currencyService) : base(context)
        {
            this.taxService = taxService;
            this.locationService = locationService;
            this.currencyService = currencyService;
        }
        public void Accept(List<long> quoteIds, string organisationId,string acceptBy)
        {
            using var context = Context();
            var quotes = context.Quote.Where(u => u.OrganisationId == organisationId && quoteIds.Any(t => t == u.Id) && u.StatusId==(int)QuoteEnum.Draft);
            foreach(var quote in quotes)
            {
                quote.StatusId = (int)QuoteEnum.Accepted;
                quote.AcceptedBy = acceptBy;
                quote.AcceptedDate = CurrentCambodiaTime;
            }
            context.SaveChanges();
           
        }
        public void Create(Quote quote)
        {
            using var context = Context();
            if (quote.CustomerId == 0)
            {
                throw new Exception("Customer is require");
            }
            if (quote.CurrencyId == 0)
            {
                throw new Exception("Currency is require");
            }
            if (quote.QuoteItems == null || !quote.QuoteItems.Any())
            {
                throw new Exception("Quote item is require");
            }

            if (quote.QuoteItems.Any(u => u.Quantity < 0))
            {
                throw new Exception("Quote item quantity must >0");
            }
            if (quote.QuoteItems.Any(u => u.UnitPrice < 0))
            {
                throw new Exception("Quote item unit price must >=0");
            }
            var newQuote = new Dal.Models.Quote
            {
                CustomerId = quote.CustomerId,
                CurrencyId = quote.CurrencyId,
                AcceptedBy = quote.AcceptedBy,
                BaseCurrencyExchangeRate = quote.BaseCurrencyExchangeRate,
                Created = CurrentCambodiaTime,
                CreatedBy = quote.CreatedBy,
                Date = quote.Date,
                ExpireDate = quote.ExpireDate,
                Note = quote.Note,
                StatusId = quote.StatusId,
                OrganisationId = quote.OrganisationId,
                RefNo = quote.RefNo,
                Number = GetQuoteNumber(context, quote.OrganisationId),
                TermAndCondition=quote.TermAndCondition
            };
            if (quote.StatusId == (int)QuoteEnum.Accepted)
            {
                newQuote.AcceptedDate = CurrentCambodiaTime;
            }
            if (quote.Attachments != null && quote.Attachments.Any())
            {
                foreach (var attachment in quote.Attachments)
                {
                    newQuote.QuoteAttachment.Add(new Dal.Models.QuoteAttachment
                    {
                        FileUrl = attachment.FileUrl,
                        IsFinalOfficalFile=attachment.IsFinalOfficalFile,
                        FileName=attachment.FileName
                    });
                }
            }
            var taxCurrency = GetTaxCurrency(context, quote.OrganisationId);
            var baseCurrencyId = GetOrganisationBaseCurrencyId(context, quote.OrganisationId);
            if (baseCurrencyId == quote.CurrencyId)
            {
                newQuote.BaseCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = quote.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == baseCurrencyId);
                newQuote.BaseCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            if (taxCurrency.Id == quote.CurrencyId)
            {
                newQuote.TaxCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = quote.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == taxCurrency.Id);
                newQuote.TaxCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            foreach (var orderItem in quote.QuoteItems)
            {
                decimal lineTotal = orderItem.Quantity * orderItem.UnitPrice;
                if (orderItem.DiscountRate != null)
                {
                    lineTotal -= (lineTotal * (decimal)orderItem.DiscountRate.Value) / 100;
                }
                var lineTotalIncludeTax = lineTotal;
                if (orderItem.TaxId != null)
                {
                    var totalTaxRate = context.TaxComponent.Where(u => u.TaxId == orderItem.TaxId).Sum(u => u.Rate);
                    lineTotalIncludeTax += (lineTotalIncludeTax * totalTaxRate) / 100;
                }
                newQuote.QuoteItem.Add(new Dal.Models.QuoteItem
                {
                    DiscountRate = orderItem.DiscountRate,
                    Quantity = orderItem.Quantity,
                    TaxId = orderItem.TaxId,
                    UnitPrice = orderItem.UnitPrice,
                    LineTotal = lineTotal,
                    LineTotalIncludeTax = lineTotalIncludeTax,
                    Description = orderItem.Product.ProductSaleInformation.Description,
                    ProductId = orderItem.ProductId,
                    LocationId = orderItem.LocationId
                });
                newQuote.Total += lineTotal;
                newQuote.TotalIncludeTax += lineTotalIncludeTax;
            }
            context.Quote.Add(newQuote);
            context.SaveChanges();
            quote.Id = newQuote.Id;
            quote.Number = GetQuoteNumber(quote.OrganisationId);
        }
        public void Decline(List<long> quoteIds, string organisationId, string declinedBy)
        {
            using var context = Context();
            var quotes = context.Quote.Where(u => u.OrganisationId == organisationId && quoteIds.Any(t => t == u.Id) && u.StatusId == (int)QuoteEnum.Draft);
            foreach (var quote in quotes)
            {
                quote.StatusId = (int)QuoteEnum.Declined;
                quote.DeclinedBy = declinedBy;
                quote.DeclinedDate = CurrentCambodiaTime;
            }
            context.SaveChanges();
        }
        public void Delete(List<long> quoteIds, string organisationId, string deleteBy)
        {
            using var context = Context();
            var quotes = context.Quote.Where(u => u.OrganisationId == organisationId && quoteIds.Any(t => t == u.Id) && (u.StatusId == (int)QuoteEnum.Draft||u.StatusId==(int)QuoteEnum.Accepted));
            foreach (var quote in quotes)
            {
                quote.StatusId = (int)QuoteEnum.Delete;
                quote.DeletedBy = deleteBy;
                quote.DeletedDate = CurrentCambodiaTime;
            }
            context.SaveChanges();
        }
        public QuoteForUpdateOrCreate GetForUpdate(string organisationId, long id)
        {
            using var context = Context();
            var quoteDb = context.Quote.FirstOrDefault(u => u.Id == id && u.OrganisationId == organisationId&&u.StatusId!=(int)QuoteEnum.Delete);
            if (quoteDb == null)
            {
                throw new Exception("Quote not found");
            }
            var taxes = taxService.GetTaxesIncludeComponent(organisationId);
            var currencies = currencyService.GetCurrenciesWithExchangeRate(organisationId);
            currencies = currencies.Where(u => u.Id != quoteDb.CurrencyId).ToList();
            var quoteCurrency = new Currency
            {
                Id = quoteDb.CurrencyId,
                Code = quoteDb.Currency.Code,
                Name = quoteDb.Currency.Name,
                Symbole = quoteDb.Currency.Symbole,
                ExchangeRates = new List<CurrencyExchangeRate>()
            };
            var baseCurrency = GetOrganisationBaseCurrency(organisationId);
            var taxCurrency = GetTaxCurrency(organisationId);

            if (quoteDb.CurrencyId != baseCurrency.Id)
            {
                quoteCurrency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                    ExchangeRate = quoteDb.BaseCurrencyExchangeRate.Value,
                    CurrencyId = baseCurrency.Id,
                    Currency = baseCurrency
                });
            }
            if (quoteDb.CurrencyId != taxCurrency.Id)
            {
                quoteCurrency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                    ExchangeRate = quoteDb.TaxCurrencyExchangeRate.Value,
                    CurrencyId = taxCurrency.Id,
                    Currency = taxCurrency
                });
            }
            currencies.Add(quoteCurrency);
            var quote = new Quote
            {
                OrganisationId = quoteDb.OrganisationId,
                AcceptedBy = quoteDb.AcceptedBy,
                AcceptedDate = quoteDb.AcceptedDate,
                Attachments = quoteDb.QuoteAttachment.Select(u => new Attachment { FileName=u.FileName,IsFinalOfficalFile=u.IsFinalOfficalFile,FileUrl=u.FileUrl}).ToList(),
                BaseCurrencyExchangeRate = quoteDb.BaseCurrencyExchangeRate,
                Created = quoteDb.Created,
                CreatedBy = quoteDb.CreatedBy,
                CurrencyId = quoteDb.CurrencyId,
                CustomerId = quoteDb.CustomerId,
                Date = quoteDb.Date,
                DeclinedBy = quoteDb.DeclinedBy,
                DeclinedDate = quoteDb.DeclinedDate,
                ExpireDate = quoteDb.ExpireDate,
                Id = quoteDb.Id,
                InvoicedBy = quoteDb.InvoicedBy,
                Note = quoteDb.Note,
                Number = quoteDb.Number,
                RefNo = quoteDb.RefNo,
                Status = new QuoteStatus
                {
                    Id = quoteDb.StatusId,
                    Name =IsKhmer?quoteDb.Status.NameKh: quoteDb.Status.Name,
                },
                StatusId = quoteDb.StatusId,
                TaxCurrencyExchangeRate = quoteDb.TaxCurrencyExchangeRate,
                Total = quoteDb.Total,
                TotalIncludeTax = quoteDb.TotalIncludeTax,
                InvoicedDate = quoteDb.InvoicedDate,
                TermAndCondition=quoteDb.TermAndCondition,
                QuoteItems = quoteDb.QuoteItem.Select(u => new QuoteItem
                {
                    Description = u.Description,
                    DiscountRate = u.DiscountRate,
                    Id = u.Id,
                    LineTotal = u.LineTotal,
                    LineTotalIncludeTax = u.LineTotalIncludeTax,
                    LocationId = u.LocationId,
                    ProductId = u.ProductId,
                    Quantity = u.Quantity,
                    TaxId = u.TaxId,
                    UnitPrice = u.UnitPrice,
                    Tax = u.Tax == null ? null : taxes.FirstOrDefault(t => t.Id == u.TaxId),
                    Product = new Product
                    {
                        Name = u.Product.Code + " - " + u.Product.ProductSaleInformation.Title,
                        Code = u.Product.Code,
                        ImageUrl = u.Product.ImageUrl,
                        TrackInventory = u.Product.ProductInventory != null,
                        ProductSaleInformation = new ProductSalePurchaseDetail
                        {
                            Description = u.Product.ProductSaleInformation.Description,
                            Price = u.UnitPrice,
                            TaxId = u.TaxId,
                            Title=u.Product.ProductSaleInformation.Title
                        }
                    }
                }).ToList()
            };
            return new QuoteForUpdateOrCreate
            {
                TaxCurrency = taxCurrency,
                BaseCurrencyId = baseCurrency.Id,
                Taxes = taxes,
                Locations = locationService.GetLocations(organisationId),
                Currencies = currencies,
                Quote = quote


            };
        }
        public QuoteForUpdateOrCreate GetLookupForCreae(string organisationId)
        {
            using var context = Context();
            return new QuoteForUpdateOrCreate
            {
                TaxCurrency = GetTaxCurrency(organisationId),
                BaseCurrencyId = GetOrganisationBaseCurrencyId(organisationId),
                Taxes = taxService.GetTaxesIncludeComponent(organisationId),
                Locations = locationService.GetLocations(organisationId),
                Currencies = currencyService.GetCurrenciesWithExchangeRate(organisationId),
                Number = GetQuoteNumber(context, organisationId)

            };
        }
        public string GetQuoteNumber(string organisationId)
        {
            return GetQuoteNumber(Context(), organisationId);
        }
        public string GetQuoteNumber(Dal.Models.InvoiceContext context, string organisationId)
        {
            var orderNum = context.Quote.Where(u => u.OrganisationId == organisationId).Count() + 1;
            return "QU-" + orderNum.ToString("000000");
        }
        public List<Quote> GetQuotes(QuoteFilter filter)
        {
            var khmer = IsKhmer;
            using var context = Context();
            var quotes = context.Quote.Where(u => u.OrganisationId == filter.OrganisationId &&
                                                        (filter.StatusId == 0 || u.StatusId == filter.StatusId) &&
                                                        (filter.CustomerId == 0 || u.CustomerId == filter.CustomerId) &&
                                                        (
                                                            (filter.DateTypeFilter.Id == (int)QuoteFilterEnum.All &&
                                                            (filter.FromDate == null || u.Date >= filter.FromDate || u.ExpireDate >= filter.FromDate) &&
                                                            (filter.ToDate == null || u.Date <= filter.ToDate || u.ExpireDate <= filter.ToDate)) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)QuoteFilterEnum.Date &&
                                                                (filter.FromDate == null || u.Date >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.Date <= filter.ToDate)
                                                            ) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)QuoteFilterEnum.Expire &&
                                                                (filter.FromDate == null || u.ExpireDate >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.ExpireDate <= filter.ToDate)
                                                            )
                                                        ) &&
                                                        (u.StatusId!=(int)QuoteEnum.Delete)&&
                                                        (string.IsNullOrEmpty(filter.SearchText) || u.Number.Contains(filter.SearchText)) 
                                                       ).OrderByDescending(u => u.Number)
                                                       .Skip((filter.Page - 1) * filter.PageSize)
                                                       .Take(filter.PageSize)
                                                       .Select(u => new Quote
                                                       {
                                                           AcceptedBy = u.AcceptedBy,
                                                           CustomerId = u.CustomerId,
                                                           StatusId = u.StatusId,
                                                           AcceptedDate = u.AcceptedDate,
                                                           BaseCurrencyExchangeRate = u.BaseCurrencyExchangeRate,
                                                           Created = u.Created,
                                                           CreatedBy = u.CreatedBy,
                                                           Currency = new Currency
                                                           {
                                                               Code = u.Currency.Code,
                                                               Name = u.Currency.Name,
                                                               Id = u.CurrencyId,
                                                               Symbole = u.Currency.Symbole,
                                                           },
                                                           CurrencyId = u.CurrencyId,
                                                           Customer = new Customer
                                                           {
                                                               Id = u.CustomerId,
                                                               DisplayName = u.Customer.DisplayName
                                                           },
                                                           Date = u.Date,
                                                           DeclinedBy = u.DeclinedBy,
                                                           Id = u.Id,
                                                           DeclinedDate = u.DeclinedDate,
                                                           ExpireDate = u.ExpireDate,
                                                           InvoicedBy = u.InvoicedBy,
                                                           InvoicedDate = u.InvoicedDate,
                                                           Note = u.Note,
                                                           Number = u.Number,
                                                           OrganisationId = u.OrganisationId,
                                                           RefNo = u.RefNo,
                                                           Status = new QuoteStatus
                                                           {
                                                               Id = u.StatusId,
                                                               Name =IsKhmer?u.Status.NameKh: u.Status.Name
                                                           },
                                                           TaxCurrencyExchangeRate = u.TaxCurrencyExchangeRate,
                                                           Total = u.Total,
                                                           TotalIncludeTax = u.TotalIncludeTax
                                                       }).ToList();
            return quotes;
        }
        public List<QuoteStatus> GetQuoteStatuses(QuoteFilter filter)
        {
            using var context = Context();
            var totalQuote = context.Quote.Where(u => u.OrganisationId == filter.OrganisationId &&
                                                       (filter.CustomerId == 0 || u.CustomerId == filter.CustomerId) &&
                                                        (
                                                            (filter.DateTypeFilter.Id == (int)QuoteFilterEnum.All &&
                                                            (filter.FromDate == null || u.Date >= filter.FromDate || u.ExpireDate >= filter.FromDate) &&
                                                            (filter.ToDate == null || u.Date <= filter.ToDate || u.ExpireDate <= filter.ToDate)) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)QuoteFilterEnum.Date &&
                                                                (filter.FromDate == null || u.Date >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.Date <= filter.ToDate)
                                                            ) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)QuoteFilterEnum.Expire &&
                                                                (filter.FromDate == null || u.ExpireDate >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.ExpireDate <= filter.ToDate)
                                                            )
                                                        ) &&
                                                       (u.StatusId != (int)QuoteEnum.Delete) &&
                                                       (string.IsNullOrEmpty(filter.SearchText) || u.Number.Contains(filter.SearchText))
                                                      ).GroupBy(u => u.StatusId)
                                                      .Select(u => new
                                                      {
                                                          StatusId = u.Key,
                                                          Count = u.Count()
                                                      }).ToList();
            var status = context.QuoteStatus.Where(u=>u.Id!=(int)QuoteEnum.Delete).ToList().Select(u => new QuoteStatus
            {
                Id = u.Id,
                Name =IsKhmer?u.NameKh: u.Name,
                Count = totalQuote.Any(t => t.StatusId == u.Id) ? totalQuote.FirstOrDefault(t => t.StatusId == u.Id).Count : 0
            }).ToList();
            status.Insert(0, new QuoteStatus
            {
                Id = 0,
                Name = AppResource.GetResource("All"),
                Count = totalQuote.Sum(u => u.Count)
            });
            return status;
        }
        public void GetTotalPages(QuoteFilter filter)
        {
            using var context = Context();
            filter.TotalRow = context.Quote.Where(u => u.OrganisationId == filter.OrganisationId &&
                                                        (filter.StatusId == 0 || u.StatusId == filter.StatusId) &&
                                                        (filter.CustomerId == 0 || u.CustomerId == filter.CustomerId) &&
                                                        (u.StatusId != (int)QuoteEnum.Delete) &&
                                                         (
                                                            (filter.DateTypeFilter.Id == (int)QuoteFilterEnum.All &&
                                                            (filter.FromDate == null || u.Date >= filter.FromDate || u.ExpireDate >= filter.FromDate) &&
                                                            (filter.ToDate == null || u.Date <= filter.ToDate || u.ExpireDate <= filter.ToDate)) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)QuoteFilterEnum.Date &&
                                                                (filter.FromDate == null || u.Date >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.Date <= filter.ToDate)
                                                            ) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)QuoteFilterEnum.Expire &&
                                                                (filter.FromDate == null || u.ExpireDate >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.ExpireDate <= filter.ToDate)
                                                            )
                                                        ) &&
                                                        (string.IsNullOrEmpty(filter.SearchText) || u.Number.Contains(filter.SearchText))
                                                       ).Count();
        }
        public void RemoveAttachment(long purchaseOrderId, string organisationId, string fileUrl)
        {
            using var context = Context();
            var quote = context.Quote.FirstOrDefault(u => u.OrganisationId == organisationId && u.Id == purchaseOrderId);
            if (quote == null)
            {
                throw new Exception("Quote not found");
            }

            if (quote.StatusId != (int)QuoteEnum.Draft)
            {
                throw new Exception("Quote status doesn't allow to remove attachment");
            }
            context.QuoteAttachment.Remove(context.QuoteAttachment.FirstOrDefault(u => u.QuoteId == purchaseOrderId && u.FileUrl == fileUrl));
            context.SaveChanges();
        }
        public void Update(Quote quote)
        {
            using var context = Context();
            if (quote.CustomerId == 0)
            {
                throw new Exception("Customer is require");
            }
            if (quote.CurrencyId == 0)
            {
                throw new Exception("Currency is require");
            }
            if (quote.QuoteItems == null || !quote.QuoteItems.Any())
            {
                throw new Exception("Quote item is require");
            }

            if (quote.QuoteItems.Any(u => u.Quantity < 0))
            {
                throw new Exception("Quote item quantity must >0");
            }
            if (quote.QuoteItems.Any(u => u.UnitPrice < 0))
            {
                throw new Exception("Quote item unit price must >=0");
            }
            var updateQuote = context.Quote.FirstOrDefault(u => u.Id == quote.Id && u.OrganisationId == quote.OrganisationId);
            if (updateQuote == null)
            {
                throw new Exception("Quote not found.");
            }
            if (updateQuote.StatusId != (int)QuoteEnum.Draft)
            {
                throw new Exception("Quote doesn't allow to update");
            }
            updateQuote.CustomerId = quote.CustomerId;
            updateQuote.CurrencyId = quote.CurrencyId;
            updateQuote.Date = quote.Date;
            updateQuote.ExpireDate = quote.ExpireDate;
            updateQuote.Note = quote.Note;
            updateQuote.StatusId = quote.StatusId;
            updateQuote.RefNo = quote.RefNo;
            updateQuote.Total = 0;
            updateQuote.TotalIncludeTax = 0;
            updateQuote.TermAndCondition = quote.TermAndCondition;
            if (quote.StatusId == (int)QuoteEnum.Accepted)
            {
                quote.AcceptedDate = CurrentCambodiaTime;
            }
            var taxCurrency = GetTaxCurrency(context, quote.OrganisationId);
            var baseCurrencyId = GetOrganisationBaseCurrencyId(context, quote.OrganisationId);
            if (baseCurrencyId == quote.CurrencyId)
            {
                updateQuote.BaseCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = quote.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == baseCurrencyId);
                updateQuote.BaseCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            if (taxCurrency.Id == quote.CurrencyId)
            {
                updateQuote.TaxCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = quote.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == taxCurrency.Id);
                updateQuote.TaxCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }

            foreach (var orderItem in quote.QuoteItems)
            {
               
                decimal lineTotal = orderItem.Quantity * orderItem.UnitPrice;
                if (orderItem.DiscountRate != null)
                {
                    lineTotal -= (lineTotal * (decimal)orderItem.DiscountRate.Value) / 100;
                }
                var lineTotalIncludeTax = lineTotal;
                if (orderItem.TaxId != null)
                {
                    var totalTaxRate = context.TaxComponent.Where(u => u.TaxId == orderItem.TaxId).Sum(u => u.Rate);
                    lineTotalIncludeTax += (lineTotalIncludeTax * totalTaxRate) / 100;
                }
                if (orderItem.Id == 0)
                {
                    updateQuote.QuoteItem.Add(new Dal.Models.QuoteItem
                    {
                        DiscountRate = orderItem.DiscountRate,
                        Quantity = orderItem.Quantity,
                        TaxId = orderItem.TaxId,
                        UnitPrice = orderItem.UnitPrice,
                        LineTotal = lineTotal,
                        LineTotalIncludeTax = lineTotalIncludeTax,
                        Description = orderItem.Product.ProductSaleInformation.Description,
                        ProductId = orderItem.ProductId,
                        LocationId = orderItem.LocationId
                    });
                }
                else
                {
                    var updateOrderItem = context.QuoteItem.FirstOrDefault(u => u.Id == orderItem.Id);
                    updateOrderItem.DiscountRate = orderItem.DiscountRate;
                    updateOrderItem.Quantity = orderItem.Quantity;
                    updateOrderItem.TaxId = orderItem.TaxId;
                    updateOrderItem.UnitPrice = orderItem.UnitPrice;
                    updateOrderItem.Description = orderItem.Product.ProductSaleInformation.Description;
                    updateOrderItem.ProductId = orderItem.ProductId;
                    updateOrderItem.LocationId = orderItem.LocationId;
                    updateOrderItem.LineTotal = lineTotal;
                    updateOrderItem.LineTotalIncludeTax = lineTotalIncludeTax;
                }
                var updateItemIds = quote.QuoteItems.Where(u => u.Id != 0).Select(u => u.Id).ToList();
                var removeQuoteItems = context.QuoteItem.Where(u => u.QuoteId == quote.Id && !updateItemIds.Any(t => u.Id == t));
                if (removeQuoteItems.Any())
                {
                    context.QuoteItem.RemoveRange(removeQuoteItems);
                }
                updateQuote.Total += lineTotal;
                updateQuote.TotalIncludeTax += lineTotalIncludeTax;
            }
            context.SaveChanges();
        }
        public string UploadAttachemnt(long purchaseOrderId, string organisationId, string baseString, string fileName)
        {
            using var context = Context();
            var quote = context.Quote.FirstOrDefault(u => u.OrganisationId == organisationId && u.Id == purchaseOrderId);
            if (quote == null)
            {
                throw new Exception("Quote not found");
            }

            if (quote.StatusId != (int)QuoteEnum.Draft)
            {
                throw new Exception("Quote status doesn't allow to add attachment");
            }
            var fileUrl = UploadFile(baseString,fileName);
            quote.QuoteAttachment.Add(new Dal.Models.QuoteAttachment
            {
                FileUrl = fileUrl,
                FileName=fileName
            }) ;
            context.SaveChanges();
            return fileUrl;
        }
        public List<StatusOverview> GetStatusOverviews(string organisationId)
        {
            using var context = Context();
            var khmer = IsKhmer;
            var organisationCurrencySymbole = context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId).BaseCurrency.Symbole;
            var bills = context.Quote.Where(u => u.OrganisationId == organisationId && (u.StatusId !=(int)QuoteEnum.Declined && u.StatusId!=(int)QuoteEnum.Delete && u.StatusId!=(int)QuoteEnum.Invoiced))
                .Select(u => new { u.StatusId, u.BaseCurrencyExchangeRate, u.TotalIncludeTax })
                .GroupBy(u => u.StatusId)
                       .Select(u => new
                       {
                           StatusId = u.Key,
                           Total = u.Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate),
                           Count = u.Count()
                       }).ToList();
            var status = (from st in context.QuoteStatus.Where(u => u.Id != (int)QuoteEnum.Declined && u.Id != (int)QuoteEnum.Delete && u.Id != (int)QuoteEnum.Invoiced)
                                    .Select(u => new { u.Id,Name=khmer?u.NameKh: u.Name }).ToList()
                          let bill = bills.FirstOrDefault(u => u.StatusId == st.Id)
                          select new StatusOverview
                          {
                              Count = bill == null ? 0 : bill.Count,
                              StatusId = st.Id,
                              StatusName = st.Name,
                              CurrencySymbole = organisationCurrencySymbole,
                              Total = bill == null ? 0 : bill.Total.Value
                          }).ToList();
            var now = CurrentCambodiaTime;
            var totalOverDue = context.Quote.Where(u => u.OrganisationId == organisationId && (u.StatusId == (int)QuoteEnum.Draft || u.StatusId == (int)QuoteEnum.Accepted && u.ExpireDate < now))
                .Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate.Value);
            var countOverDue = context.Quote.Where(u => u.OrganisationId == organisationId && (u.StatusId == (int)QuoteEnum.Draft || u.StatusId == (int)QuoteEnum.Accepted && u.ExpireDate < now)).Count();
            status.Add(new StatusOverview
            {
                Count = countOverDue,
                CurrencySymbole = organisationCurrencySymbole,
                StatusId = (int)QuoteEnum.Expire,
                StatusName = AppResource.GetResource("Expire"),
                Total = totalOverDue
            });
            return status;
        }

        public void ChangeOfficialDocument(long quoteId, string organisationId, string fileUrl)
        {
            using var context = Context();
            var attachmentFiles = context.QuoteAttachment.Where(u => u.QuoteId == quoteId && u.Quote.OrganisationId == organisationId && u.FileUrl != fileUrl && u.IsFinalOfficalFile == true).ToList();
            foreach (var attachment in attachmentFiles)
            {
                attachment.IsFinalOfficalFile = false;
            }
            var t = context.QuoteAttachment.FirstOrDefault(u => u.QuoteId == quoteId && u.FileUrl == fileUrl);
            t.IsFinalOfficalFile = true;
            context.SaveChanges();
        }
    }
}
