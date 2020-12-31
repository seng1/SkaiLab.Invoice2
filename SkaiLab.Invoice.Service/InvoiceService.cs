using Microsoft.EntityFrameworkCore.Internal;
using SkaiLab.Invoice.Dal.Models;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.Xml;

namespace SkaiLab.Invoice.Service
{
    public class InvoiceService:Service,IInvoiceService
    {
        public InvoiceService(IDataContext context) : base(context)
        {

        }
        public void Create(Models.Invoice invoice)
        {
            using var context = Context();
            if (invoice.CustomerId == 0)
            {
                throw new Exception("Customer is require");
            }
            if(invoice.CustomerTransactionItems==null || !invoice.CustomerTransactionItems.Any())
            {
                throw new Exception("Invoice item is require");
            }
            if (invoice.CustomerTransactionItems.Any(u => u.Quantity <= 0))
            {
                throw new Exception("Invoice item quantity must >0");
            }
            if (invoice.CustomerTransactionItems.Any(u => u.UnitPrice < 0))
            {
                throw new Exception("Invoice item unit price must <=0");
            }
            if(invoice.CustomerTransactionItems.Any(u=>u.DiscountRate!=null&&(u.DiscountRate<0|| u.DiscountRate > 100)))
            {
                throw new Exception("Invoice item discount rate must between 0 and 100");
            }
            if (invoice.CustomerTransactionItems.Any(u => u.Product.TrackInventory && u.LocationId == null))
            {
                throw new Exception("Invoice item location is require");
            }
            
            var newInvoice = new Dal.Models.Invoice
            {
                StatusId = (int)InvoiceStatusEnum.WaitingForPayment,
                IdNavigation = new Dal.Models.CustomerTransaction
                {
                    Created = CurrentCambodiaTime,
                    CreatedBy = invoice.CreatedBy,
                    CurrencyId = invoice.CurrencyId,
                    CustomerId = invoice.CustomerId,
                    Date = invoice.Date,
                    DueDate = invoice.DueDate,
                    IsTaxIncome = invoice.CustomerTransactionItems.Any(u => u.TaxId != null),
                    Note = invoice.Note,
                    PaidBy = "",
                    RefNo = invoice.RefNo,
                    PaidDate = null,
                    OrganisationId = invoice.OrganisationId,
                    Total = 0,
                    TotalIncludeTax = 0,
                    TermAndCondition=invoice.TermAndCondition

                }
            };
            newInvoice.IdNavigation.Number = CreateInvoiceNumber(context, invoice.OrganisationId, invoice.IsTaxIncome, invoice.Date.Year, invoice.Date.Month);
            var taxCurrency = GetTaxCurrency(context, invoice.OrganisationId);
            var baseCurrencyId = GetOrganisationBaseCurrencyId(context, invoice.OrganisationId);
            if (baseCurrencyId == invoice.CurrencyId)
            {
                newInvoice.IdNavigation.BaseCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = invoice.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == baseCurrencyId);
                newInvoice.IdNavigation.BaseCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            if (taxCurrency.Id == invoice.CurrencyId)
            {
                newInvoice.IdNavigation.TaxCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = invoice.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == taxCurrency.Id);
                newInvoice.IdNavigation.TaxCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            if (invoice.Attachments != null && invoice.Attachments.Any())
            {
                foreach(var attachment in invoice.Attachments)
                {
                    newInvoice.IdNavigation.CustomerTransactionAttachment.Add(new CustomerTransactionAttachment
                    {
                        FileUrl=attachment.FileUrl,
                        FileName=attachment.FileName,
                        IsFinalOfficalFile=attachment.IsFinalOfficalFile
                    });
                }
            }
            foreach (var orderItem in invoice.CustomerTransactionItems)
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
                newInvoice.IdNavigation.CustomerTransactionItem.Add(new Dal.Models.CustomerTransactionItem
                {
                    DiscountRate = orderItem.DiscountRate,
                    Quantity = orderItem.Quantity,
                    TaxId = orderItem.TaxId,
                    UnitPrice = orderItem.UnitPrice,
                    LineTotal = lineTotal,
                    LineTotalIncludeTax = lineTotalIncludeTax,
                    Description = orderItem.Description,
                    CustomerTransactionItemProduct = new CustomerTransactionItemProduct
                    {
                        ProductId = orderItem.ProductId.Value,
                        LocationId = orderItem.LocationId
                    }
                });
                newInvoice.IdNavigation.Total += lineTotal;
                newInvoice.IdNavigation.TotalIncludeTax += lineTotalIncludeTax;

            }
            if (!OutInventory(context, invoice.CustomerTransactionItems, newInvoice.IdNavigation.Number, newInvoice.IdNavigation.Date, invoice.CreatedBy, newInvoice.IdNavigation.BaseCurrencyExchangeRate))
            {
                throw new Exception($"Product doesn't have enough inventory.");
            }
            context.Invoice.Add(newInvoice);
            context.SaveChanges();
            invoice.Id = newInvoice.Id;
            invoice.Number = newInvoice.IdNavigation.Number;
        }
        bool OutInventory(InvoiceContext context, List<Models.CustomerTransactionItem> customerTransactionItems, string refNo,DateTime date,string createdBy,decimal baseExchangeRate)
        {
            var inventoriesOrderItems = customerTransactionItems.Where(u => u.Product.TrackInventory)
               .GroupBy(u => new { u.ProductId, u.LocationId })
               .Select(u => u);
            if (!inventoriesOrderItems.Any())
            {
                return true;
            }
            var isSuccess = true;
            foreach (var inventoryOrderItem in inventoriesOrderItems)
            {

                var totalQty = inventoryOrderItem.Sum(u => u.Quantity);
                var inventoryBalance = context.ProductInventoryBalance.FirstOrDefault(u => u.ProductId == inventoryOrderItem.Key.ProductId && u.LocationId == inventoryOrderItem.Key.LocationId);
                
                if (inventoryBalance == null || inventoryBalance.Quantity < totalQty)
                {
                    isSuccess = false;
                    break;
                }
                var inventories = (from inventoryIn in context.ProductInventoryHistoryIn
                                   where inventoryIn.IdNavigation.ProductId == inventoryOrderItem.Key.ProductId &&
                                          inventoryIn.IdNavigation.LocationId == inventoryOrderItem.Key.LocationId
                                          && (!inventoryIn.ProductInventoryHistoryOut.Any()||inventoryIn.IdNavigation.Quantity > inventoryIn.ProductInventoryHistoryOut.Sum(u => u.IdNavigation.Quantity))
                                   orderby inventoryIn.IdNavigation.Date
                                   select inventoryIn).ToList();
                var dicInventory = new Dictionary<long, int>();
                foreach (var orderItem in inventoryOrderItem)
                {
                    var qty = orderItem.Quantity;
                    foreach (var inventory in inventories)
                    {
                        var rmnQty = inventory.IdNavigation.Quantity - inventory.ProductInventoryHistoryOut.Sum(u => u.IdNavigation.Quantity);
                        var outQty = rmnQty >= qty ? qty : rmnQty;
                        if (dicInventory.ContainsKey(inventory.Id))
                        {
                            outQty -= dicInventory[inventory.Id];
                        }
                        if (outQty > 0)
                        {
                            qty -= outQty;
                            inventory.ProductInventoryHistoryOut.Add(new ProductInventoryHistoryOut
                            {
                                IdNavigation = new ProductInventoryHistory
                                {
                                    Quantity = outQty,
                                    RefNo = refNo,
                                    CreatedBy = createdBy,
                                    Created = CurrentCambodiaTime,
                                    Date =date,
                                    Description = $"Invoice {refNo}",
                                    LocationId = inventory.IdNavigation.LocationId,
                                    ProductId = inventory.IdNavigation.ProductId,
                                    UnitPrice = (orderItem.LineTotal / orderItem.Quantity) * baseExchangeRate
                                }
                            });
                            dicInventory.Add(inventory.Id, outQty);
                            if (qty == 0)
                            {
                                break;
                            }
                        }

                    }
                    if (qty > 0)
                    {
                        isSuccess = false;
                        break;
                    }
                }
                inventoryBalance.Quantity -= totalQty;

            }
            return isSuccess;

        }

        public void CreateInvoiceFromQuote(Models.Invoice invoice)
        {
            using var context = Context();
            if (invoice.CustomerId == 0)
            {
                throw new Exception("Customer is require");
            }
            if(invoice.CustomerTransactionItems.Any(u=>u.Product.TrackInventory&& u.LocationId == null))
            {
                throw new Exception("Location is require");
            }
            var quote = context.Quote.FirstOrDefault(u => u.Id == invoice.QuoteId);
            if (quote == null)
            {
                throw new Exception("Quote is require");
            }
            if (quote.StatusId != (int)QuoteEnum.Accepted)
            {
                throw new Exception("Quote status doesn't allow to generate invoice");
            }
            var newInvoice =new Dal.Models.Invoice{
                StatusId=(int) InvoiceStatusEnum.WaitingForPayment,
                InvoiceQuote=new InvoiceQuote
                {
                    QuoteId=quote.Id,
                },
                IdNavigation=new Dal.Models.CustomerTransaction
                {
                    Created=CurrentCambodiaTime,
                    CreatedBy=invoice.CreatedBy,
                    CurrencyId=invoice.CurrencyId,
                    CustomerId=invoice.CustomerId,
                    Date=invoice.Date,
                    DueDate=invoice.DueDate,
                    IsTaxIncome=invoice.CustomerTransactionItems.Any(u=>u.TaxId!=null),
                    Note=invoice.Note,
                    PaidBy="",
                    RefNo=invoice.RefNo,
                    PaidDate=null,
                    OrganisationId=invoice.OrganisationId,
                    Total=0,
                    TotalIncludeTax=0,
                    TermAndCondition = invoice.TermAndCondition

                }
            };
            newInvoice.IdNavigation.Number = CreateInvoiceNumber(context, invoice.OrganisationId,invoice.IsTaxIncome, invoice.Date.Year, invoice.Date.Month);
            var taxCurrency = GetTaxCurrency(context, quote.OrganisationId);
            var baseCurrencyId = GetOrganisationBaseCurrencyId(context, quote.OrganisationId);
            if (baseCurrencyId == invoice.CurrencyId)
            {
                newInvoice.IdNavigation.BaseCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = invoice.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == baseCurrencyId);
                newInvoice.IdNavigation.BaseCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            if (taxCurrency.Id == quote.CurrencyId)
            {
                newInvoice.IdNavigation.TaxCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = invoice.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == taxCurrency.Id);
                newInvoice.IdNavigation.TaxCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            foreach (var orderItem in invoice.CustomerTransactionItems)
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
                newInvoice.IdNavigation.CustomerTransactionItem.Add(new Dal.Models.CustomerTransactionItem
                {
                    DiscountRate = orderItem.DiscountRate,
                    Quantity = orderItem.Quantity,
                    TaxId = orderItem.TaxId,
                    UnitPrice = orderItem.UnitPrice,
                    LineTotal = lineTotal,
                    LineTotalIncludeTax = lineTotalIncludeTax,
                    Description = orderItem.Description,
                    CustomerTransactionItemProduct=new CustomerTransactionItemProduct
                    {
                        ProductId=orderItem.ProductId.Value,
                        LocationId=orderItem.LocationId
                    }
                });
                newInvoice.IdNavigation.Total += lineTotal;
                newInvoice.IdNavigation.TotalIncludeTax += lineTotalIncludeTax;
               
            }
            if (invoice.Attachments != null && invoice.Attachments.Any())
            {
                foreach (var attachment in invoice.Attachments)
                {
                    newInvoice.IdNavigation.CustomerTransactionAttachment.Add(new CustomerTransactionAttachment
                    {
                        FileUrl = attachment.FileUrl,
                        IsFinalOfficalFile=attachment.IsFinalOfficalFile,
                        FileName=attachment.FileName
                    });
                }
            }
            if (!OutInventory(context, invoice.CustomerTransactionItems, newInvoice.IdNavigation.Number, newInvoice.IdNavigation.Date, invoice.CreatedBy, newInvoice.IdNavigation.BaseCurrencyExchangeRate))
            {
                throw new Exception($"Product doesn't have enough inventory.");
            }
            quote.StatusId = (int)QuoteEnum.Invoiced;
            quote.InvoicedBy = invoice.CreatedBy;
            quote.InvoicedDate = CurrentCambodiaTime;
            context.Invoice.Add(newInvoice);
            context.SaveChanges();
            invoice.Id = newInvoice.Id;
            invoice.Number = newInvoice.IdNavigation.Number;
        }

        public string CreateInvoiceNumber(string organisationId, bool taxInvoice, int year,int month)
        {
            return CreateInvoiceNumber(Context(), organisationId, taxInvoice, year, month);
        }

        public string CreateInvoiceNumber(InvoiceContext context, string organisationId, bool taxInvoice, int year,int month)
        {
            var totalInvoice = context.Invoice.Where(u => u.IdNavigation.OrganisationId == organisationId && u.IdNavigation.IsTaxIncome == taxInvoice && u.IdNavigation.Date.Year == year).Count();
            totalInvoice += 1;
            if (taxInvoice)
            {
                return $"{year:0000}-{month:00}-{totalInvoice:00000}";
            }
            return $"N{year:0000}-{month:00}-{totalInvoice:00000}";
        }

        public Models.Invoice GetInvoiceFromQuote(long quoteId, string organsationId)
        {
            using var context = Context();
            var quote = context.Quote.FirstOrDefault(u => u.Id == quoteId && u.OrganisationId == organsationId);
            if (quote == null)
            {
                throw new Exception("Quote not found");
            }
            if (quote.StatusId !=(int) QuoteEnum.Accepted)
            {
                throw new Exception($"Quote {quote.Number} status doesn't allow to generate invoice.");
            }
            var invoice = new Models.Invoice
            {

                Attachments = quote.QuoteAttachment.Select(u => new Attachment
                {
                    FileUrl=u.FileUrl,
                    IsFinalOfficalFile=u.IsFinalOfficalFile,
                    FileName=u.FileName
                }).ToList(),
                BaseCurrencyExchangeRate = quote.BaseCurrencyExchangeRate.Value,
                Created = CurrentCambodiaTime,
                CreatedBy = "",
                CurrencyId = quote.CurrencyId,
                Currency = new Models.Currency
                {
                    Id = quote.CurrencyId,
                    Code = quote.Currency.Code,
                    Name = quote.Currency.Name,
                    Symbole=quote.Currency.Symbole,
                    ExchangeRates=new List<CurrencyExchangeRate>()
                },
                CustomerId = quote.CustomerId,
                Customer = new Models.Customer
                {
                    Id = quote.CurrencyId,
                    DisplayName = quote.Customer.DisplayName
                },
                Date = CurrentCambodiaTime,
                DueDate = null,
                Id = 0,
                Note = quote.Note,
                OrganisationId = quote.OrganisationId,
                PaidBy = "",
                PaidDate = null,
                QuoteId = quote.Id,
                RefNo = quote.RefNo,
                StatusId = 0,
                Status = null,
                Total = quote.Total,
                TotalIncludeTax = quote.TotalIncludeTax,
                TaxCurrencyExchangeRate = quote.TaxCurrencyExchangeRate.Value,
                CustomerTransactionItems = quote.QuoteItem.Select(u => new Models.CustomerTransactionItem
                {
                    Description=u.Description,
                    DiscountRate=u.DiscountRate,
                    Id=0,
                    LineTotal=u.LineTotal,
                    LineTotalIncludeTax=u.LineTotalIncludeTax,
                    LocationId=u.LocationId,
                    ProductId=u.ProductId,
                    Product=new Models.Product
                    {
                        Id=u.ProductId,
                        Code=u.Product.Code,
                        Name=u.Product.Name,
                        ProductSaleInformation=new ProductSalePurchaseDetail
                        {
                             Description=u.Product.ProductSaleInformation.Description,
                             Title=u.Product.ProductSaleInformation.Title
                        },
                        ImageUrl=u.Product.ImageUrl, 
                        TrackInventory=u.Product.ProductInventory!=null
                    },
                    Tax=u.TaxId==null?new Models.Tax():new Models.Tax
                    {
                        Id=u.Tax.Id,
                        Name=u.Tax.Name,
                        Components=u.Tax.TaxComponent.Select(t=>new Models.TaxComponent
                        {
                            Id=t.Id,
                            Name=t.Name,
                            Rate=t.Rate
                        }).ToList()
                    },
                    TaxId=u.TaxId,
                    Quantity=u.Quantity,
                    UnitPrice=u.UnitPrice
                }).ToList()
            };
            invoice.IsTaxIncome = invoice.CustomerTransactionItems.Any(u => u.TaxId != null);
            invoice.Number = CreateInvoiceNumber(context, quote.OrganisationId, invoice.IsTaxIncome, invoice.Date.Year, invoice.Date.Month);
            var baseCurrency = GetOrganisationBaseCurrency(invoice.OrganisationId);
            var taxCurrency = GetTaxCurrency(invoice.OrganisationId);
            if (invoice.Currency.Id != baseCurrency.Id)
            {
                invoice.Currency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                    CurrencyId = baseCurrency.Id,
                    Currency = baseCurrency,
                    ExchangeRate = invoice.BaseCurrencyExchangeRate
                });
            }
            if (invoice.Currency.Id != taxCurrency.Id)
            {
                invoice.Currency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                   CurrencyId=taxCurrency.Id,
                   Currency=taxCurrency,
                   ExchangeRate=invoice.TaxCurrencyExchangeRate
                });
            }

            return invoice;
        }

        public List<Models.Invoice> GetInvoices(InvoiceFilter filter)
        {
            using var context = Context();
            var khmer = IsKhmer;
            var result = context.Invoice.Where(u => (filter.StatusId == 0 || u.StatusId == filter.StatusId)
                                     && (u.IdNavigation.OrganisationId == filter.OrganisationId)
                                     && (filter.CustomerId == 0 || u.IdNavigation.CustomerId == filter.CustomerId)&&
                                   (
                                        (filter.DateTypeFilter.Id == (int)InvoiceDateTypeFilterEnum.All &&
                                        (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate || u.IdNavigation.DueDate >= filter.FromDate) &&
                                        (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate || u.IdNavigation.DueDate <= filter.ToDate)) ||
                                         (filter.DateTypeFilter.Id == (int)InvoiceDateTypeFilterEnum.Date &&
                                         (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate) &&
                                         (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate) ) ||
                                        (
                                            filter.DateTypeFilter.Id == (int)InvoiceDateTypeFilterEnum.DueDate &&
                                            (filter.FromDate == null || u.IdNavigation.DueDate >= filter.FromDate) &&
                                            (filter.ToDate == null || u.IdNavigation.DueDate <= filter.ToDate))
                                     ) 
                                     && (string.IsNullOrEmpty(filter.SearchText) || u.IdNavigation.Number.Contains(filter.SearchText))
                                    ).OrderByDescending(u => u.IdNavigation.Date)
                                    .Skip((filter.Page - 1) * filter.PageSize)
                                    .Take(filter.PageSize)
                                    .Select(u => new Models.Invoice
                                    {
                                        BaseCurrencyExchangeRate=u.IdNavigation.BaseCurrencyExchangeRate,
                                        Created=u.IdNavigation.Created,
                                        CreatedBy=u.IdNavigation.CreatedBy,
                                        Currency=new Models.Currency
                                        {
                                            Code=u.IdNavigation.Currency.Code,
                                             Id=u.IdNavigation.CurrencyId,
                                             Name=u.IdNavigation.Currency.Name,
                                             Symbole=u.IdNavigation.Currency.Symbole
                                        },
                                        CurrencyId=u.IdNavigation.CurrencyId,
                                        Id=u.Id,
                                        Customer=new Models.Customer
                                        {
                                            Id=u.IdNavigation.CustomerId,
                                            DisplayName=u.IdNavigation.Customer.DisplayName
                                        },
                                        CustomerId=u.IdNavigation.CustomerId,
                                        Date=u.IdNavigation.Date,
                                        DueDate=u.IdNavigation.DueDate,
                                        IsTaxIncome=u.IdNavigation.IsTaxIncome,
                                        Note=u.IdNavigation.Note,
                                        Number=u.IdNavigation.Number,
                                        OrganisationId=u.IdNavigation.OrganisationId,
                                        RefNo=u.IdNavigation.RefNo,
                                        StatusId=u.StatusId,
                                        Total=u.IdNavigation.Total,
                                        TotalIncludeTax=u.IdNavigation.TotalIncludeTax,
                                        TaxCurrencyExchangeRate=u.IdNavigation.TaxCurrencyExchangeRate,
                                        Status=new Models.InvoiceStatus
                                        {
                                            Id=u.StatusId,
                                            Name=khmer?u.Status.NameKh:u.Status.Name
                                        },
                                        PaidBy=u.IdNavigation.PaidBy,
                                        PaidDate=u.IdNavigation.PaidDate

                                    }).ToList();
            return result;
        }

        public List<Models.InvoiceStatus> GetInvoiceStatuses(InvoiceFilter filter)
        {
            using var context = Context();
            var result = context.Invoice.Where(u => (u.IdNavigation.OrganisationId==filter.OrganisationId)
                                  && (filter.CustomerId == 0 || u.IdNavigation.CustomerId == filter.CustomerId)
                                          && (
                                        (filter.DateTypeFilter.Id == (int)InvoiceDateTypeFilterEnum.All &&
                                        (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate || u.IdNavigation.DueDate >= filter.FromDate) &&
                                        (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate || u.IdNavigation.DueDate <= filter.ToDate)) ||
                                         (filter.DateTypeFilter.Id == (int)InvoiceDateTypeFilterEnum.Date &&
                                         (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate) &&
                                         (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate)) ||
                                        (
                                            filter.DateTypeFilter.Id == (int)InvoiceDateTypeFilterEnum.DueDate &&
                                            (filter.FromDate == null || u.IdNavigation.DueDate >= filter.FromDate) &&
                                            (filter.ToDate == null || u.IdNavigation.DueDate <= filter.ToDate))
                                     )
                                  && (string.IsNullOrEmpty(filter.SearchText) || u.IdNavigation.Number.Contains(filter.SearchText))
                                  ).GroupBy(u => u.StatusId)
                                  .Select(u => new
                                  {
                                      StatusId = u.Key,
                                      Count = u.Count()
                                  }).ToList();
            var statues = context.InvoiceStatus.ToList()
                .Select(u => new Models.InvoiceStatus
                {
                    Count=result.Any(t=>t.StatusId==u.Id)?result.FirstOrDefault(t=>t.StatusId==u.Id).Count:0,
                    Id=u.Id,
                    Name=IsKhmer?u.NameKh: u.Name
                }).ToList();
            statues.Insert(0, new Models.InvoiceStatus
            {
                Id = 0,
                Name = AppResource.GetResource("All"),
                Count = result.Sum(u => u.Count)
            });
            return statues;
        }

        public void GetTotalPages(InvoiceFilter filter)
        {
            using var context = Context();
            filter.TotalRow = context.Invoice.Where(u => (filter.StatusId == 0 || u.StatusId == filter.StatusId)
                                     && (u.IdNavigation.OrganisationId == filter.OrganisationId)
                                     && (filter.CustomerId == 0 || u.IdNavigation.CustomerId == filter.CustomerId)
                                     &&(
                                        (filter.DateTypeFilter.Id == (int)InvoiceDateTypeFilterEnum.All &&
                                        (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate || u.IdNavigation.DueDate >= filter.FromDate) &&
                                        (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate || u.IdNavigation.DueDate <= filter.ToDate)) ||
                                         (filter.DateTypeFilter.Id == (int)InvoiceDateTypeFilterEnum.Date &&
                                         (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate) &&
                                         (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate)) ||
                                        (
                                            filter.DateTypeFilter.Id == (int)InvoiceDateTypeFilterEnum.DueDate &&
                                            (filter.FromDate == null || u.IdNavigation.DueDate >= filter.FromDate) &&
                                            (filter.ToDate == null || u.IdNavigation.DueDate <= filter.ToDate))
                                     )
                                     && (string.IsNullOrEmpty(filter.SearchText) || u.IdNavigation.Number.Contains(filter.SearchText))
                                    ).Count();
        }

        public Models.Invoice GetInvoice(long id, string organisationId)
        {
            using var context = Context();
            var invoice = context.Invoice.FirstOrDefault(u => u.Id == id && u.IdNavigation.OrganisationId == organisationId).IdNavigation;
            var result = new Models.Invoice
            {
                Attachments=invoice.CustomerTransactionAttachment.Select(u=>new Attachment { FileName=u.FileName,IsFinalOfficalFile=u.IsFinalOfficalFile,FileUrl=u.FileUrl}).ToList(),
                BaseCurrencyExchangeRate=invoice.BaseCurrencyExchangeRate,
                Created=invoice.Created,
                CreatedBy=invoice.CreatedBy,
                Currency=new Models.Currency
                {
                    Code=invoice.Currency.Code,
                    ExchangeRates=new List<CurrencyExchangeRate>(),
                    Id=invoice.CurrencyId,
                    Name=invoice.Currency.Name,
                    Symbole=invoice.Currency.Symbole
                },
                CurrencyId=invoice.CurrencyId,
                CustomerId=invoice.CustomerId,
                Customer=new Models.Customer
                {
                    Id=invoice.CustomerId,
                    DisplayName=invoice.Customer.DisplayName
                },
                Date=invoice.Date,
                DueDate=invoice.DueDate,
                Id=invoice.Id,
                IsTaxIncome=invoice.IsTaxIncome,
                Note=invoice.Note,
                Number=invoice.Number,
                OrganisationId=invoice.OrganisationId,
                PaidBy=invoice.PaidBy,
                PaidDate=invoice.PaidDate,
                RefNo=invoice.RefNo,
                TermAndCondition=invoice.TermAndCondition,
                Status=new Models.InvoiceStatus
                {
                    Id=invoice.Invoice.StatusId,
                    Name=IsKhmer?invoice.Invoice.Status.NameKh: invoice.Invoice.Status.Name
                },
                StatusId=invoice.Invoice.StatusId,
                TaxCurrencyExchangeRate=invoice.TaxCurrencyExchangeRate,
                Total=invoice.Total,
                TotalIncludeTax=invoice.TotalIncludeTax,
                CustomerTransactionItems=invoice.CustomerTransactionItem.Select(u=>new Models.CustomerTransactionItem
                {
                    Description=u.Description,
                    DiscountRate=u.DiscountRate,
                    Id=u.Id,
                     LineTotal=u.LineTotal,
                     LineTotalIncludeTax=u.LineTotalIncludeTax,
                     Location=u.CustomerTransactionItemProduct.LocationId==null?new Models.Location():new Models.Location
                     {
                         Id=u.CustomerTransactionItemProduct.Location.Id,
                         Name=u.CustomerTransactionItemProduct.Location.Name
                     },
                     LocationId=u.CustomerTransactionItemProduct.LocationId,
                     Product=new Models.Product
                     {
                         Id=u.CustomerTransactionItemProduct.ProductId,
                         Code=u.CustomerTransactionItemProduct.Product.Code,
                         Name=u.Description,
                         ImageUrl=u.CustomerTransactionItemProduct.Product.ImageUrl,
                         ProductSaleInformation=new ProductSalePurchaseDetail
                         {
                             Title=u.CustomerTransactionItemProduct.Product.ProductSaleInformation.Title,
                             Description=u.CustomerTransactionItemProduct.Product.ProductSaleInformation.Description
                         },
                         TrackInventory=u.CustomerTransactionItemProduct.Product.ProductInventory!=null
                     },
                     ProductId=u.CustomerTransactionItemProduct.ProductId,
                     Quantity=u.Quantity,
                     TaxId=u.TaxId,
                     Tax=u.Tax==null?new Models.Tax():new Models.Tax
                     {
                         Id=u.Tax.Id,
                         Name=u.Tax.Name,
                         Components=u.Tax.TaxComponent.Select(t=>new Models.TaxComponent
                         {
                             Id=t.Id,
                             Name=t.Name,
                             Rate=t.Rate
                         }).ToList(),
                         TotalRate=u.Tax.TaxComponent.Sum(u=>u.Rate)
                     },
                     UnitPrice=u.UnitPrice
                }).ToList()
            };
            var taxCurrency = GetTaxCurrency(result.OrganisationId);
            var baseCurrency = GetOrganisationBaseCurrency(result.OrganisationId);
            if (result.CurrencyId != taxCurrency.Id)
            {
                result.Currency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                    Currency=taxCurrency,
                    CurrencyId=taxCurrency.Id,
                    ExchangeRate=result.TaxCurrencyExchangeRate
                });
            }
            if (result.CurrencyId != baseCurrency.Id)
            {
                result.Currency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                    Currency = baseCurrency,
                    CurrencyId = baseCurrency.Id,
                    ExchangeRate = result.BaseCurrencyExchangeRate
                });
            }
            return result;
        }
        public List<StatusOverview> GetStatusOverviews(string organisationId)
        {
            using var context = Context();
            var organisationCurrencySymbole = context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId).BaseCurrency.Symbole;
            var khmer = IsKhmer;

            var bills = context.Invoice.Where(u => u.IdNavigation.OrganisationId == organisationId && u.StatusId == (int)InvoiceStatusEnum.WaitingForPayment)
                .Select(u => new { u.StatusId, u.IdNavigation.BaseCurrencyExchangeRate, u.IdNavigation.TotalIncludeTax })
                .GroupBy(u => u.StatusId)
                       .Select(u => new
                       {
                           StatusId = u.Key,
                           Total = u.Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate),
                           Count = u.Count()
                       }).ToList();
            var status = (from st in context.InvoiceStatus.Where(u => u.Id == (int)InvoiceStatusEnum.WaitingForPayment)
                                    .Select(u => new { u.Id,Name= khmer?u.NameKh: u.Name }).ToList()
                          let bill = bills.FirstOrDefault(u => u.StatusId == st.Id)
                          select new StatusOverview
                          {
                              Count = bill == null ? 0 : bill.Count,
                              StatusId = st.Id,
                              StatusName = st.Name,
                              CurrencySymbole = organisationCurrencySymbole,
                              Total = bill == null ? 0 : bill.Total
                          }).ToList();
            var now = CurrentCambodiaTime;
            var totalOverDue = context.Invoice.Where(u => u.IdNavigation.OrganisationId == organisationId && (u.StatusId == (int)InvoiceStatusEnum.WaitingForPayment && u.IdNavigation.DueDate < now))
                .Sum(u => u.IdNavigation.TotalIncludeTax * u.IdNavigation.BaseCurrencyExchangeRate);
            var countOverDue = context.Invoice.Where(u => u.IdNavigation.OrganisationId == organisationId && (u.StatusId == (int)InvoiceStatusEnum.WaitingForPayment && u.IdNavigation.DueDate < now)).Count();
            status.Add(new StatusOverview
            {
                Count = countOverDue,
                CurrencySymbole = organisationCurrencySymbole,
                StatusId = (int)InvoiceStatusEnum.OverDue,
                StatusName =AppResource.GetResource("Overdue"),
                Total = totalOverDue
            });
            return status;
        }

        public void Pay(long id, string organisationId,string paidBy)
        {
            using var context = Context();
            var invoice = context.Invoice.FirstOrDefault(u => u.Id == id && u.IdNavigation.OrganisationId == organisationId);
            if (invoice == null)
            {
                throw new Exception("Invoice not found");
            }
            if (invoice.StatusId == (int)InvoiceStatusEnum.Paid)
            {
                throw new Exception("Invoice already paid");
            }
            invoice.StatusId= (int)InvoiceStatusEnum.Paid;
            invoice.IdNavigation.PaidDate = CurrentCambodiaTime;
            invoice.IdNavigation.PaidBy = paidBy;
            context.SaveChanges();
        }

        public void Pay(List<long> ids, string organisationId, string paidBy)
        {
            using var context = Context();
            var allInvoices = context.Invoice.Where(u => u.IdNavigation.OrganisationId == organisationId && u.StatusId == (int)InvoiceStatusEnum.WaitingForPayment && ids.Any(t => t == u.Id)).ToList();
            if (allInvoices.Count != ids.Count)
            {
                throw new Exception("Some invoice already paid");
            }
            foreach(var invoice in allInvoices)
            {
                invoice.StatusId = (int)InvoiceStatusEnum.Paid;
                invoice.IdNavigation.PaidDate = CurrentCambodiaTime;
                invoice.IdNavigation.PaidBy = paidBy;
            }
            context.SaveChanges();
        }

        public void UploadFile(long id, string organisationId, Attachment attachment)
        {
            using var context = Context();
            var invoice = context.Invoice.FirstOrDefault(u => u.Id == id && u.IdNavigation.OrganisationId == organisationId);
            if (invoice == null)
            {
                throw new Exception("Invoice not found");
            }
            attachment.FileUrl = UploadFile(attachment.FileUrl, attachment.FileName);
            invoice.IdNavigation.CustomerTransactionAttachment.Add(new CustomerTransactionAttachment
            {
                FileName=attachment.FileName,
                FileUrl=attachment.FileUrl,
                IsFinalOfficalFile=attachment.IsFinalOfficalFile
            });
            context.SaveChanges();
        }
        public void ChangeOfficialDocument(long id, string organisationId, string fileUrl)
        {
            using var context = Context();
            var attachmentFiles = context.CustomerTransactionAttachment.Where(u => u.CustomerTransactionId == id && u.CustomerTransaction.OrganisationId == organisationId && u.FileUrl != fileUrl && u.IsFinalOfficalFile == true).ToList();
            foreach (var attachment in attachmentFiles)
            {
                attachment.IsFinalOfficalFile = false;
            }
            var t = context.CustomerTransactionAttachment.FirstOrDefault(u => u.CustomerTransactionId == id && u.FileUrl == fileUrl);
            t.IsFinalOfficalFile = true;
            context.SaveChanges();
        }
    }
}
