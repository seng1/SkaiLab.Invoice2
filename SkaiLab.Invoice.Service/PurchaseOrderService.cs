using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class PurchaseOrderService : ExpenseService, IPurchaseOrderService
    {
        public PurchaseOrderService(IDataContext context) : base(context)
        {

        }

        public void Create(PurchaseOrder purchaseOrder)
        {
            using var context = Context();
            if (purchaseOrder.VendorId == 0)
            {
                throw new Exception("Vendor is require");
            }
            if (purchaseOrder.CurrencyId == 0)
            {
                throw new Exception("Currency is require");
            }
            if(purchaseOrder.ExpenseItems==null || !purchaseOrder.ExpenseItems.Any())
            {
                throw new Exception("Purchase order item is require");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.ProductId == 0))
            {
                throw new Exception("Product is require");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.Quantity < 0))
            {
                throw new Exception("Purchase order item quantity must >0");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.UnitPrice < 0))
            {
                throw new Exception("Purchase order item unit price must >=0");
            }
            if(purchaseOrder.ExpenseItems.Any(u=>u.DiscountRate!=null && (u.DiscountRate<0 || u.DiscountRate > 100)))
            {
                throw new Exception("Purchase order item discount rate must between 0 and 100");
            }
            var newOrder = new Dal.Models.Expense
            {
                ApprovedBy = "",
                ApprovedDate = null,
                BilledBy = "",
                BilledDate = null,
                Created = CurrentCambodiaTime,
                CreatedBy = purchaseOrder.CreatedBy,
                CurrencyId = purchaseOrder.CurrencyId,
                DeliveryDate = purchaseOrder.DeliveryDate,
                Date = purchaseOrder.Date,
                Note = purchaseOrder.Note,
                Number = GetPurchaseOrderNumber(context, purchaseOrder.OrganisationId),
                OrganisationId = purchaseOrder.OrganisationId,
                ExpenseStatusId = purchaseOrder.ExpenseStatusId,
                RefNo = purchaseOrder.RefNo,
                VendorId = purchaseOrder.VendorId,
                Total = 0,
                TotalIncludeTax = 0,
                PurchaseOrder = new Dal.Models.PurchaseOrder(),
                TermAndCondition=purchaseOrder.TermAndCondition

            };
            if(purchaseOrder.Attachments!=null&& purchaseOrder.Attachments.Any())
            {
                foreach(var attachment in purchaseOrder.Attachments)
                {
                    newOrder.ExpenseAttachentFile.Add(new Dal.Models.ExpenseAttachentFile
                    {
                        FileUrl = attachment.FileUrl,
                        IsFinalOfficalFile=attachment.IsFinalOfficalFile,
                        FileName=attachment.FileName
                    });
                }
            }
            var taxCurrency= GetTaxCurrency(context, purchaseOrder.OrganisationId);
            var baseCurrencyId = GetOrganisationBaseCurrencyId(context, purchaseOrder.OrganisationId);
            if (baseCurrencyId == purchaseOrder.CurrencyId)
            {
                newOrder.BaseCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = purchaseOrder.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == baseCurrencyId);
                newOrder.BaseCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            if (taxCurrency.Id == purchaseOrder.CurrencyId)
            {
                newOrder.TaxCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = purchaseOrder.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == taxCurrency.Id);
                newOrder.TaxCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            foreach(var orderItem in purchaseOrder.ExpenseItems)
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
                newOrder.ExpenseItem.Add(new Dal.Models.ExpenseItem
                {
                    DiscountRate = orderItem.DiscountRate,
                    Quantity = orderItem.Quantity,
                    TaxId = orderItem.TaxId,
                    UnitPrice = orderItem.UnitPrice,
                    LineTotal = lineTotal,
                    LineTotalIncludeTax=lineTotalIncludeTax,
                    ExpenseProductItem=new Dal.Models.ExpenseProductItem
                    {
                        ProductId = orderItem.ProductId.Value,
                        LocationId = orderItem.LocationId,
                    },
                    Description=orderItem.Product.ProductPurchaseInformation.Description
                });
                newOrder.Total += lineTotal;
                newOrder.TotalIncludeTax += lineTotalIncludeTax;
            }
            
            if (purchaseOrder.ExpenseStatusId == (int)ExpenseStatusEnum.Approved)
            {
                if (purchaseOrder.CloseDate == null)
                {
                    throw new Exception("Close date is require");
                }
                newOrder.ApprovedBy = purchaseOrder.ApprovedBy;
                newOrder.ApprovedDate = CurrentCambodiaTime;
                newOrder.CloseDate = purchaseOrder.CloseDate;
                var productIds = purchaseOrder.ExpenseItems.Select(u => u.ProductId).Distinct();
                var productInventoryIds = context.ProductInventory.Where(u => productIds.Any(t => t == u.Id))
                    .Select(u => u.Id).ToList();

                if (purchaseOrder.ExpenseItems.Any(u => u.LocationId == null && productInventoryIds.Any(t=>t==u.ProductId)))
                {
                    throw new Exception("Purchase order item location is require");
                }
                var inventoryBalancesLocal = new List<Dal.Models.ProductInventoryBalance>();
                foreach(var purchaseOrderItem in purchaseOrder.ExpenseItems)
                {
                    if (productInventoryIds.Any(t=>t==purchaseOrderItem.ProductId))
                    {
                        var inventoryBalance = context.ProductInventoryBalance.FirstOrDefault(u => u.ProductId == purchaseOrderItem.ProductId && u.LocationId == purchaseOrderItem.LocationId);
                        if (inventoryBalance == null)
                        {
                            inventoryBalance = inventoryBalancesLocal.FirstOrDefault(u => u.LocationId == purchaseOrderItem.LocationId && u.ProductId == purchaseOrderItem.ProductId);
                            if (inventoryBalance == null)
                            {
                                inventoryBalance = new Dal.Models.ProductInventoryBalance
                                {
                                    LocationId = purchaseOrderItem.LocationId.Value,
                                    Quantity = 0,
                                    ProductId = purchaseOrderItem.ProductId.Value
                                };
                                context.ProductInventoryBalance.Add(inventoryBalance);
                                inventoryBalancesLocal.Add(inventoryBalance);
                            }
                        }
                        inventoryBalance.Quantity += purchaseOrderItem.Quantity;
                        context.ProductInventoryHistory.Add(new Dal.Models.ProductInventoryHistory
                        {
                            LocationId = purchaseOrderItem.LocationId.Value,
                            ProductId = purchaseOrderItem.ProductId.Value,
                            Quantity = purchaseOrderItem.Quantity,
                            Created = CurrentCambodiaTime,
                            Date = purchaseOrder.Date,
                            CreatedBy = purchaseOrder.CreatedBy,
                            Description = "Purchase " + purchaseOrder.OrderNumber,
                            RefNo = purchaseOrder.OrderNumber,
                            UnitPrice = purchaseOrderItem.UnitPrice * newOrder.BaseCurrencyExchangeRate.Value,
                            ProductInventoryHistoryIn = new Dal.Models.ProductInventoryHistoryIn
                            {

                            }
                        });
                    }
                 
                }
            }
            context.Expense.Add(newOrder);
            context.SaveChanges();
            purchaseOrder.Id = newOrder.Id;
            purchaseOrder.OrderNumber = newOrder.Number;
        }

        public List<ExpenseStatus> GetExpenseStatuses(PurchaseOrderFilter filter)
        {
            var context = Context();
            var statuses = context.PurchaseOrder.Where(u => u.IdNavigation.OrganisationId == filter.OrganisationId &&
                                                            u.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.Delete &&
                                                           (filter.VendorId == 0 || u.IdNavigation.VendorId == filter.VendorId) &&
                                                          (
                                                            (filter.DateTypeFilter.Id == (int)PurchaseOrderDateTypeFilter.All &&
                                                            (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate || u.IdNavigation.DeliveryDate >= filter.FromDate) &&
                                                            (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate || u.IdNavigation.DeliveryDate <= filter.ToDate)) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)PurchaseOrderDateTypeFilter.Date &&
                                                                (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate)
                                                            ) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)PurchaseOrderDateTypeFilter.DeliveryDate &&
                                                                (filter.FromDate == null || u.IdNavigation.DeliveryDate >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.IdNavigation.DeliveryDate <= filter.ToDate)
                                                            )
                                                        ) &&
                                                           (string.IsNullOrEmpty(filter.SearchText) || u.IdNavigation.Number.Contains(filter.SearchText))
                                                         )
                            .GroupBy(u => new { u.IdNavigation.ExpenseStatusId })
                            .Select(u => new 
                            {
                                Id=u.Key.ExpenseStatusId,
                                Count=u.Count()
                            }).ToList();
            var result = context.ExpenseStatus.Where(u=>u.Id!=(int)ExpenseStatusEnum.Delete).ToList()
                .Select(u => new ExpenseStatus
                {
                    Id=u.Id,
                    Name=IsKhmer?u.NameKh: u.Name,
                    Count= statuses.Any(c=>c.Id==u.Id)? statuses.FirstOrDefault(c=>c.Id==u.Id).Count:0
                }).ToList();
            result.Insert(0, new ExpenseStatus
            {
                Id=0,
                Name=AppResource.GetResource("All"),
                Count=statuses.Sum(u=>u.Count)
            });
            return result;
        }

        public PurchaseOrder GetPurchase(string organisationId, long id)
        {
            using var context = Context();
            var order = context.PurchaseOrder.FirstOrDefault(u => u.Id == id && u.IdNavigation.OrganisationId == organisationId);
            if (order == null)
            {
                throw new Exception("Purchase order not found");
            }
            //return new PurchaseOrder
            //{
            //     ApprovedBy=order.ApprovedBy,
            //     ApprovedDate=order.ApprovedDate,
            //     BaseCurrencyExchangeRate=order.BaseCurrencyExchangeRate,
            //     BilledBy=order.BilledBy,
            //     BilledDate=order.BilledDate,
            //     Created=order.Created,
            //     CreatedBy=order.CreatedBy,
            //     Currency=new Currency
            //     {
            //         Code=order.Currency.Code,
            //         Name=order
            //     }
            //}
            return null;
        }

        public PurchaseOrderForUpdate GetPurchaseOrderForUpdate(string organisationId, long id)
        {
            using var context = Context();
            var order = context.PurchaseOrder.FirstOrDefault(u => u.Id == id && u.IdNavigation.OrganisationId == organisationId);
            if (order == null)
            {
                throw new Exception("Purchase order not found");
            }
            var locations = context.Location.Where(u => u.OrganisationId == organisationId)
                .OrderBy(u => u.Name)
                .Select(u => new Location
                {
                    Name=u.Name,
                    Id=u.Id
                }).ToList();
            var currencies = GetCurrenciesWithExchangeRate(context, organisationId);
            currencies = currencies.Where(u => u.Id != order.IdNavigation.CurrencyId).ToList();
            var baseCurrency = GetOrganisationBaseCurrency(organisationId);
            var taxCurrency = GetTaxCurrency(organisationId);
            var orderCurrency = new Currency
            {
                Id = order.IdNavigation.CurrencyId,
                Code = order.IdNavigation.Currency.Code,
                Name = order.IdNavigation.Currency.Name,
                Symbole = order.IdNavigation.Currency.Symbole,
                ExchangeRates=new List<CurrencyExchangeRate>()
            };
            currencies.Add(orderCurrency);
            if (order.IdNavigation.CurrencyId != baseCurrency.Id)
            {
                orderCurrency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                     ExchangeRate=order.IdNavigation.BaseCurrencyExchangeRate.Value,
                     CurrencyId=baseCurrency.Id,
                     Currency=baseCurrency
                });
            }
            if (order.IdNavigation.CurrencyId != taxCurrency.Id)
            {
                orderCurrency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                    ExchangeRate = order.IdNavigation.TaxCurrencyExchangeRate.Value,
                    CurrencyId = taxCurrency.Id,
                    Currency=taxCurrency
                });
            }
            var taxes = GetTaxesIncludeComponent(context, organisationId);
            var purchaseOrder = new PurchaseOrder
            {
                BaseCurrencyExchangeRate= order.IdNavigation.BaseCurrencyExchangeRate.Value,
                 Currency=currencies.FirstOrDefault(u=>u.Id==order.IdNavigation.CurrencyId),
                 CurrencyId=order.IdNavigation.CurrencyId,
                 OrderNumber=order.IdNavigation.Number,
                 Id=order.Id,
                 Date=order.IdNavigation.Date,
                 DeliveryDate=order.IdNavigation.DeliveryDate,
                  Note=order.IdNavigation.Note,
                  RefNo=order.IdNavigation.RefNo,
                  VendorId=order.IdNavigation.VendorId,
                 Total=order.IdNavigation.Total,
                 TotalIncludeTax=order.IdNavigation.TotalIncludeTax,
                 TaxCurrencyExchangeRate=order.IdNavigation.TaxCurrencyExchangeRate.Value,
                 ExpenseStatusId=order.IdNavigation.ExpenseStatusId,
                 TermAndCondition=order.IdNavigation.TermAndCondition,
                 ExpenseStatus=new ExpenseStatus
                 {
                     Id=order.IdNavigation.ExpenseStatusId,
                     Name=IsKhmer?order.IdNavigation.ExpenseStatus.NameKh: order.IdNavigation.ExpenseStatus.Name
                 },
                 OrganisationId=order.IdNavigation.OrganisationId,
                 BilledBy=order.IdNavigation.BilledBy,
                 ApprovedBy=order.IdNavigation.ApprovedBy,
                 ApprovedDate=order.IdNavigation.ApprovedDate,
                 BilledDate=order.IdNavigation.BilledDate,
                  Created=order.IdNavigation.Created,
                  CreatedBy=order.IdNavigation.CreatedBy,
                  Attachments=order.IdNavigation.ExpenseAttachentFile.Select(u=>new Attachment { FileName=u.FileName,IsFinalOfficalFile=u.IsFinalOfficalFile.Value,FileUrl=u.FileUrl}).ToList(),
                ExpenseItems = order.IdNavigation.ExpenseItem.ToList().Select(u=>new PurchaseOrderItem
                  {
                      DiscountRate=u.DiscountRate,
                      Id=u.Id,
                      LineTotal=u.LineTotal,
                      LineTotalIncludeTax=u.LineTotalIncludeTax,
                      LocationId=u.ExpenseProductItem.LocationId,
                      ProductId=u.ExpenseProductItem.ProductId,
                      UnitPrice=u.UnitPrice,
                      Quantity=u.Quantity,
                      TaxId=u.TaxId,
                      Tax=u.TaxId==null?null:taxes.FirstOrDefault(t=>t.Id==u.TaxId),
                      Product=new Product
                      {
                          Id=u.ExpenseProductItem.ProductId,
                           Code=u.ExpenseProductItem.Product.Code,
                           ImageUrl=u.ExpenseProductItem.Product.ImageUrl,
                           LocationId=u.ExpenseProductItem.Product.ProductInventory==null?null:u.ExpenseProductItem.Product.ProductInventory.DefaultLocationId,
                           Name=u.ExpenseProductItem.Product.ProductPurchaseInformation.Description,
                           ProductPurchaseInformation=new ProductSalePurchaseDetail
                           {
                               Description=u.ExpenseProductItem.Product.ProductPurchaseInformation.Description,
                               Price=u.UnitPrice*order.IdNavigation.BaseCurrencyExchangeRate.Value,
                               TaxId=u.ExpenseProductItem.Product.ProductPurchaseInformation.TaxId,
                               Title=u.ExpenseProductItem.Product.ProductPurchaseInformation.Title
                           },
                           TrackInventory=u.ExpenseProductItem.Product.ProductInventory!=null
                      }
                  }).ToList(),
                  Vendor=new Vendor
                  {
                      Id=order.IdNavigation.VendorId,
                      DisplayName=order.IdNavigation.Vendor.DisplayName
                  }
            };
            return new PurchaseOrderForUpdate
            {
                Currencies = currencies,
                Locations = locations,
                Taxes = taxes,
                PurchaseOrder=purchaseOrder,
                BaseCurrencyId=baseCurrency.Id,
                TaxCurrency=taxCurrency
            };
        }

        public string GetPurchaseOrderNumber(Dal.Models.InvoiceContext context, string organisationId)
        {
            var orderNum = context.PurchaseOrder.Where(u => u.IdNavigation.OrganisationId == organisationId).Count() + 1;
            return "PO-" + orderNum.ToString("000000");
        }
        public string GetPurchaseOrderNumber(string organisationId)
        {
            return GetPurchaseOrderNumber(Context(), organisationId);
        }

        public List<PurchaseOrder> GetPurchaseOrders(PurchaseOrderFilter filter)
        {
            var context = Context();
            var khmer = IsKhmer;
            var orders = context.PurchaseOrder.Where(u => u.IdNavigation.OrganisationId == filter.OrganisationId &&
                                                          u.IdNavigation.ExpenseStatusId!=(int)ExpenseStatusEnum.Delete&&
                                                        (filter.PurchaseOrderStatusId == 0 || u.IdNavigation.ExpenseStatusId == filter.PurchaseOrderStatusId) &&
                                                        (filter.VendorId == 0 || u.IdNavigation.VendorId == filter.VendorId) &&
                                                        (
                                                            (filter.DateTypeFilter.Id==(int)PurchaseOrderDateTypeFilter.All &&
                                                            (filter.FromDate==null || u.IdNavigation.Date>=filter.FromDate || u.IdNavigation.DeliveryDate>=filter.FromDate)&&
                                                            (filter.ToDate==null || u.IdNavigation.Date<=filter.ToDate || u.IdNavigation.DeliveryDate<=filter.ToDate))||
                                                            (
                                                                filter.DateTypeFilter.Id==(int)PurchaseOrderDateTypeFilter.Date &&
                                                                (filter.FromDate==null || u.IdNavigation.Date>=filter.FromDate) &&
                                                                (filter.ToDate==null || u.IdNavigation.Date<=filter.ToDate)
                                                            )||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)PurchaseOrderDateTypeFilter.DeliveryDate &&
                                                                (filter.FromDate == null || u.IdNavigation.DeliveryDate >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.IdNavigation.DeliveryDate <= filter.ToDate)
                                                            )
                                                        ) &&
                                                        (string.IsNullOrEmpty(filter.SearchText) || u.IdNavigation.Number.Contains(filter.SearchText))
                                                       ).OrderByDescending(u => u.IdNavigation.Number)
                                                       .Skip((filter.Page - 1) * filter.PageSize)
                                                       .Take(filter.PageSize)
                                                       .Select(u => new PurchaseOrder
                                                       {
                                                           ApprovedBy = u.IdNavigation.ApprovedBy,
                                                           ApprovedDate = u.IdNavigation.ApprovedDate,
                                                           BaseCurrencyExchangeRate = u.IdNavigation.BaseCurrencyExchangeRate.Value,
                                                           BilledBy = u.IdNavigation.BilledBy,
                                                           BilledDate = u.IdNavigation.BilledDate,
                                                           Created = u.IdNavigation.Created,
                                                           CreatedBy = u.IdNavigation.CreatedBy,
                                                           Currency = new Currency
                                                           {
                                                               Id = u.IdNavigation.CurrencyId,
                                                               Code = u.IdNavigation.Currency.Code,
                                                               Name = u.IdNavigation.Currency.Name,
                                                               Symbole = u.IdNavigation.Currency.Symbole
                                                           },
                                                           Date = u.IdNavigation.Date,
                                                           DeliveryDate = u.IdNavigation.DeliveryDate,
                                                           Note = u.IdNavigation.Note,
                                                           Id = u.Id,
                                                           CurrencyId = u.IdNavigation.CurrencyId,
                                                           OrderNumber = u.IdNavigation.Number,
                                                           OrganisationId = u.IdNavigation.OrganisationId,
                                                           ExpenseStatus = new ExpenseStatus
                                                           {
                                                               Id = u.IdNavigation.ExpenseStatusId,
                                                               Name =khmer?u.IdNavigation.ExpenseStatus.NameKh: u.IdNavigation.ExpenseStatus.Name,
                                                           },
                                                           ExpenseStatusId = u.IdNavigation.ExpenseStatusId,
                                                           RefNo = u.IdNavigation.RefNo,
                                                           TaxCurrencyExchangeRate = u.IdNavigation.TaxCurrencyExchangeRate.Value,
                                                           Total = u.IdNavigation.Total,
                                                           TotalIncludeTax = u.IdNavigation.TotalIncludeTax,
                                                           VendorId = u.IdNavigation.VendorId,
                                                           Vendor = new Vendor
                                                           {
                                                               Id=u.IdNavigation.VendorId,
                                                               DisplayName=u.IdNavigation.Vendor.DisplayName
                                                           },
                                                           CloseDate = u.IdNavigation.CloseDate
                                                       }).ToList();
            var orderIds = orders.Select(u => u.Id).ToList();
            var closeAttachments = context.ExpenseAttachentFile.Where(u => u.IsFinalOfficalFile == true && orderIds.Any(t => t == u.ExpenseId))
                .Select(u => new Attachment
                {
                    FileName=u.FileName,
                    FileUrl=u.FileUrl,
                    IsFinalOfficalFile=true,
                    ExpenseId=u.ExpenseId
                  
                }).ToList();
            foreach(var order in orders)
            {
                var closeAttachment = closeAttachments.FirstOrDefault(u => u.ExpenseId == order.Id);
                order.HasCloseDoc = closeAttachment != null;
                order.CloseAttachment = closeAttachment == null ? new Attachment() : closeAttachment;
            }
            return orders;
        }

        public void GetTotalPages(PurchaseOrderFilter filter)
        {
            var context = Context();
            filter.TotalRow = context.PurchaseOrder.Where(u => u.IdNavigation.OrganisationId == filter.OrganisationId &&
                                                            u.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.Delete &&
                                                           (filter.PurchaseOrderStatusId == 0 || u.IdNavigation.ExpenseStatusId == filter.PurchaseOrderStatusId) &&
                                                           (filter.VendorId == 0 || u.IdNavigation.VendorId == filter.VendorId) &&
                                                           (
                                                            (filter.DateTypeFilter.Id == (int)PurchaseOrderDateTypeFilter.All &&
                                                            (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate || u.IdNavigation.DeliveryDate >= filter.FromDate) &&
                                                            (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate || u.IdNavigation.DeliveryDate <= filter.ToDate)) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)PurchaseOrderDateTypeFilter.Date &&
                                                                (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate)
                                                            ) ||
                                                            (
                                                                filter.DateTypeFilter.Id == (int)PurchaseOrderDateTypeFilter.DeliveryDate &&
                                                                (filter.FromDate == null || u.IdNavigation.DeliveryDate >= filter.FromDate) &&
                                                                (filter.ToDate == null || u.IdNavigation.DeliveryDate <= filter.ToDate)
                                                            )
                                                        ) &&
                                                           (string.IsNullOrEmpty(filter.SearchText) || u.IdNavigation.Number.Contains(filter.SearchText))
                                                         ).Count();
        }

        public void MarkAsApprove(List<PurchaseOrder> purchaseOrderapproves, string organisationId, string approvedBy)
        {
            using var context = Context();
            var purchaseOrderIds = purchaseOrderapproves.Select(u => u.Id).ToList();
            var purchaseOrders = context.PurchaseOrder.Where(u => u.IdNavigation.OrganisationId == organisationId
                                  && u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.WaitingForApproval &&
                                  purchaseOrderIds.Any(t => t == u.Id))
                                 .Select(u => u).ToList();
            if (purchaseOrderIds.Count < purchaseOrderIds.Count)
            {
                throw new Exception("We cannot purchase orders to approved because some purchase order already move status.");
            }
            foreach (var purchaseOrder in purchaseOrders)
            {
                var order = purchaseOrderapproves.FirstOrDefault(u => u.Id == purchaseOrder.Id);
                purchaseOrder.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Approved;
                purchaseOrder.IdNavigation.ApprovedBy = approvedBy;
                purchaseOrder.IdNavigation.CloseDate = order.CloseDate;
                purchaseOrder.IdNavigation.ApprovedDate = CurrentCambodiaTime;
                if(!order.HasCloseDoc && !string.IsNullOrEmpty(order.CloseAttachment.FileUrl))
                {
                    purchaseOrder.IdNavigation.ExpenseAttachentFile.Add(new Dal.Models.ExpenseAttachentFile
                    {
                        FileUrl=order.CloseAttachment.FileUrl,
                        FileName=order.CloseAttachment.FileName,
                        IsFinalOfficalFile=true
                    });
                }
                var purchaseOrderItemInventories = purchaseOrder.IdNavigation.ExpenseItem.Where(u => u.ExpenseProductItem!=null &&u.ExpenseProductItem.Product.ProductInventory != null).ToList();
                if (purchaseOrderItemInventories.Any())
                {
                    if (purchaseOrderItemInventories.Any(u => u.ExpenseProductItem.LocationId == null))
                    {
                        throw new Exception($"Purchase order order item {purchaseOrder.IdNavigation.Number} does not set location yet");
                    }
                    var inventoryBalancesLocal = new List<Dal.Models.ProductInventoryBalance>();
                    foreach (var purchaseOrderItem in purchaseOrderItemInventories)
                    {
                        var inventoryBalance = context.ProductInventoryBalance.FirstOrDefault(u => u.ProductId == purchaseOrderItem.ExpenseProductItem.ProductId && u.LocationId == purchaseOrderItem.ExpenseProductItem.LocationId);
                        if (inventoryBalance == null)
                        {
                            inventoryBalance = inventoryBalancesLocal.FirstOrDefault(u => u.LocationId == purchaseOrderItem.ExpenseProductItem.LocationId && u.ProductId == purchaseOrderItem.ExpenseProductItem.ProductId);
                            if (inventoryBalance == null)
                            {
                                inventoryBalance = new Dal.Models.ProductInventoryBalance
                                {
                                    LocationId = purchaseOrderItem.ExpenseProductItem.LocationId.Value,
                                    Quantity = 0,
                                    ProductId = purchaseOrderItem.ExpenseProductItem.ProductId
                                };
                                context.ProductInventoryBalance.Add(inventoryBalance);
                                inventoryBalancesLocal.Add(inventoryBalance);
                            }
                        }
                        inventoryBalance.Quantity += purchaseOrderItem.Quantity;
                        context.ProductInventoryHistory.Add(new Dal.Models.ProductInventoryHistory
                        {
                            LocationId = purchaseOrderItem.ExpenseProductItem.LocationId.Value,
                            ProductId = purchaseOrderItem.ExpenseProductItem.ProductId,
                            Quantity = purchaseOrderItem.Quantity,
                            Created = CurrentCambodiaTime,
                            Date = purchaseOrder.IdNavigation.Date,
                            CreatedBy = purchaseOrder.IdNavigation.CreatedBy,
                            Description = "Purchase " + purchaseOrder.IdNavigation.Number,
                            RefNo = purchaseOrder.IdNavigation.Number,
                            UnitPrice = purchaseOrderItem.UnitPrice * purchaseOrder.IdNavigation.BaseCurrencyExchangeRate.Value,
                            ProductInventoryHistoryIn = new Dal.Models.ProductInventoryHistoryIn
                            {

                            }
                        });

                    }
                }
            }
            context.SaveChanges();
        }

        public void MarkAsBill(PurchaseOrder purchaseOrder)
        {
            using var context = Context();
            var purchaseOrderDb = context.PurchaseOrder.FirstOrDefault(u => u.Id == purchaseOrder.Id && u.IdNavigation.OrganisationId == purchaseOrder.OrganisationId);
            if (purchaseOrderDb == null)
            {
                throw new Exception("Purchase order not found");
            }
            if (purchaseOrderDb.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.Approved)
            {
                throw new Exception("Purchase order status does not allow to mark as bill");
            }
            purchaseOrderDb.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Billed;
            purchaseOrderDb.IdNavigation.BilledBy = purchaseOrder.BilledBy;
            purchaseOrderDb.IdNavigation.BilledDate = CurrentCambodiaTime;
            context.SaveChanges();
        }

        public void MarkAsBill(List<long> purchaseOrderIds, string organisationId, string billBy)
        {
            using var context = Context();
            var purchaseOrders = context.PurchaseOrder.Where(u => u.IdNavigation.OrganisationId == organisationId
                                  && u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Approved &&
                                  purchaseOrderIds.Any(t => t == u.Id))
                                 .Select(u => u).ToList();
            if (purchaseOrderIds.Count < purchaseOrderIds.Count)
            {
                throw new Exception("We cannot purchase orders to bill because some purchase order already move status.");
            }
            foreach (var purchaseOrder in purchaseOrders)
            {
                purchaseOrder.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Billed;
                purchaseOrder.IdNavigation.BilledBy = billBy;
                purchaseOrder.IdNavigation.BilledDate = CurrentCambodiaTime;
            }
            context.SaveChanges();
        }

        public void MarkAsDelete(List<long> purchaseOrderIds, string organisationId, string deletBy)
        {
            using var context = Context();
            var purchaseOrders = context.PurchaseOrder.Where(u => u.IdNavigation.OrganisationId == organisationId
                                  && (u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Draft||
                                  u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.WaitingForApproval) &&
                                  purchaseOrderIds.Any(t => t == u.Id))
                                 .Select(u => u).ToList();
            if (purchaseOrderIds.Count < purchaseOrderIds.Count)
            {
                throw new Exception("We cannot delete purchase orders because some purchase order already move status.");
            }
            foreach(var purchaseOrder in purchaseOrders)
            {
                purchaseOrder.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Delete;
                purchaseOrder.IdNavigation.DeletedBy = deletBy;
                purchaseOrder.IdNavigation.DeletedDate = CurrentCambodiaTime;
            }
            context.SaveChanges();
        }

        public void MarkAsWaitingForApproval(List<long> purchaseOrderIds, string organisationId)
        {
            using var context = Context();
            var purchaseOrders = context.PurchaseOrder.Where(u => u.IdNavigation.OrganisationId == organisationId
                                  && u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Draft &&
                                  purchaseOrderIds.Any(t => t == u.Id))
                                 .Select(u => u).ToList();
            if(purchaseOrderIds.Count< purchaseOrderIds.Count)
            {
                throw new Exception("We cannot purchase orders to waiting for approval because some purchase order already move status.");
            }
            foreach(var purchaseOrder in purchaseOrders)
            {
                purchaseOrder.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.WaitingForApproval;
            }
            context.SaveChanges();
                                
        }

        public void MarkDelete(PurchaseOrder purchaseOrder)
        {
            using var context = Context();
            var purchaseOrderDb = context.PurchaseOrder.FirstOrDefault(u => u.Id == purchaseOrder.Id && u.IdNavigation.OrganisationId == purchaseOrder.OrganisationId);
            if (purchaseOrderDb == null)
            {
                throw new Exception("Purchase order not found");
            }
            if(purchaseOrderDb.IdNavigation.ExpenseStatusId!=(int)ExpenseStatusEnum.WaitingForApproval
                && purchaseOrderDb.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.Draft){
                throw new Exception("Purchase order doesn't allow to delete");
            }
            purchaseOrderDb.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Delete;
            purchaseOrderDb.IdNavigation.DeletedBy = purchaseOrder.DeletedBy;
            purchaseOrderDb.IdNavigation.DeletedDate = CurrentCambodiaTime;
            context.SaveChanges();
        }


        public void Update(PurchaseOrder purchaseOrder)
        {
            using var context = Context();
            if (purchaseOrder.VendorId == 0)
            {
                throw new Exception("Vendor is require");
            }
            if (purchaseOrder.CurrencyId == 0)
            {
                throw new Exception("Currency is require");
            }
            if (purchaseOrder.ExpenseItems == null || !purchaseOrder.ExpenseItems.Any())
            {
                throw new Exception("Purchase order item is require");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.ProductId == 0))
            {
                throw new Exception("Product is require");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.Quantity < 0))
            {
                throw new Exception("Purchase order item quantity must >0");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.UnitPrice < 0))
            {
                throw new Exception("Purchase order item unit price must >=0");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.DiscountRate != null && (u.DiscountRate < 0 || u.DiscountRate > 100)))
            {
                throw new Exception("Purchase order item discount rate must between 0 and 100");
            }
            var purchaseOrderDb = context.PurchaseOrder.FirstOrDefault(u => u.Id == purchaseOrder.Id && u.IdNavigation.OrganisationId == purchaseOrder.OrganisationId);
            if (purchaseOrderDb == null)
            {
                throw new Exception("No Purchase order for update");
            }
            if(purchaseOrderDb.IdNavigation.ExpenseStatusId== (int)ExpenseStatusEnum.Approved 
                || purchaseOrderDb.IdNavigation.ExpenseStatusId==(int)ExpenseStatusEnum.Billed 
                || purchaseOrderDb.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Delete)
            {
                throw new Exception("Purchase order status doesn't allow to update");
            }
            purchaseOrderDb.IdNavigation.VendorId = purchaseOrder.VendorId;
            purchaseOrderDb.IdNavigation.Date = purchaseOrder.Date;
            purchaseOrderDb.IdNavigation.DeliveryDate = purchaseOrder.DeliveryDate;
            purchaseOrderDb.IdNavigation.RefNo = purchaseOrder.RefNo;
            purchaseOrderDb.IdNavigation.CurrencyId = purchaseOrder.CurrencyId;
            purchaseOrderDb.IdNavigation.Note = purchaseOrder.Note;
            purchaseOrderDb.IdNavigation.ExpenseStatusId = purchaseOrder.ExpenseStatusId;
            purchaseOrderDb.IdNavigation.TermAndCondition = purchaseOrder.TermAndCondition;
            var taxCurrency = GetTaxCurrency(context, purchaseOrder.OrganisationId);
            var baseCurrencyId = GetOrganisationBaseCurrencyId(context, purchaseOrder.OrganisationId);
            if (baseCurrencyId == purchaseOrder.CurrencyId)
            {
                purchaseOrderDb.IdNavigation.BaseCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = purchaseOrder.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == baseCurrencyId);
                purchaseOrderDb.IdNavigation.BaseCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            if (taxCurrency.Id == purchaseOrder.CurrencyId)
            {
                purchaseOrderDb.IdNavigation.TaxCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = purchaseOrder.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == taxCurrency.Id);
                purchaseOrderDb.IdNavigation.TaxCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            purchaseOrderDb.IdNavigation.Total = 0;
            purchaseOrderDb.IdNavigation.TotalIncludeTax = 0;
            var orderItemsIds = purchaseOrder.ExpenseItems.Where(u => u.Id != 0).Select(u => u.Id).ToList();
            var removeOrderItems = context.ExpenseItem.Where(u => u.ExpenseId == purchaseOrder.Id && !orderItemsIds.Any(t => u.Id == t));
            if (removeOrderItems.Any())
            {
                context.ExpenseProductItem.RemoveRange(removeOrderItems.Where(u => u.ExpenseProductItem != null).Select(u => u.ExpenseProductItem));
                context.ExpenseItem.RemoveRange(removeOrderItems);
            }
            foreach (var orderItem in purchaseOrder.ExpenseItems)
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
                    purchaseOrderDb.IdNavigation.ExpenseItem.Add(new Dal.Models.ExpenseItem
                    {
                        DiscountRate = orderItem.DiscountRate,
                        Quantity = orderItem.Quantity,
                        TaxId = orderItem.TaxId,
                        UnitPrice = orderItem.UnitPrice,
                        LineTotal = lineTotal,
                        LineTotalIncludeTax = lineTotalIncludeTax,
                        Description=orderItem.Product.ProductPurchaseInformation.Description,
                        ExpenseProductItem=new Dal.Models.ExpenseProductItem
                        {
                            ProductId=orderItem.ProductId.Value,
                            LocationId=orderItem.LocationId
                        }
                    });
                }
                else
                {
                    var orderItemdb = context.ExpenseItem.FirstOrDefault(u => u.Id == orderItem.Id);
                    orderItemdb.DiscountRate = orderItem.DiscountRate;
                    orderItemdb.ExpenseProductItem.ProductId = orderItem.ProductId.Value;
                    orderItemdb.TaxId = orderItem.TaxId;
                    orderItemdb.Quantity = orderItem.Quantity;
                    orderItemdb.ExpenseProductItem.LocationId = orderItem.LocationId;
                    orderItemdb.LineTotal = lineTotal;
                    orderItemdb.LineTotalIncludeTax = lineTotalIncludeTax;
                }
                purchaseOrderDb.IdNavigation.Total += lineTotal;
                purchaseOrderDb.IdNavigation.TotalIncludeTax += lineTotalIncludeTax;
            }
            if (purchaseOrder.ExpenseStatusId == (int)ExpenseStatusEnum.Approved)
            {
                if (purchaseOrder.CloseDate == null)
                {
                    throw new Exception("Close date is require");
                }
                purchaseOrderDb.IdNavigation.ApprovedBy = purchaseOrder.ApprovedBy;
                purchaseOrderDb.IdNavigation.ApprovedDate = CurrentCambodiaTime;
                purchaseOrderDb.IdNavigation.CloseDate = purchaseOrder.CloseDate;
                var productIds = purchaseOrder.ExpenseItems.Select(u => u.ProductId).Distinct();
                var productInventoryIds = context.ProductInventory.Where(u => productIds.Any(t => t == u.Id))
                    .Select(u => u.Id).ToList();

                if (purchaseOrder.ExpenseItems.Any(u => u.LocationId == null && productInventoryIds.Any(t => t == u.ProductId)))
                {
                    throw new Exception("Purchase order item location is require");
                }
                var inventoryBalancesLocal = new List<Dal.Models.ProductInventoryBalance>();
                foreach (var purchaseOrderItem in purchaseOrder.ExpenseItems)
                {
                    if (productInventoryIds.Any(t => t == purchaseOrderItem.ProductId))
                    {
                        var inventoryBalance = context.ProductInventoryBalance.FirstOrDefault(u => u.ProductId == purchaseOrderItem.ProductId && u.LocationId == purchaseOrderItem.LocationId);
                        if (inventoryBalance == null)
                        {
                            inventoryBalance = inventoryBalancesLocal.FirstOrDefault(u => u.LocationId == purchaseOrderItem.LocationId && u.ProductId == purchaseOrderItem.ProductId);
                            if (inventoryBalance == null)
                            {
                                inventoryBalance = new Dal.Models.ProductInventoryBalance
                                {
                                    LocationId = purchaseOrderItem.LocationId.Value,
                                    Quantity = 0,
                                    ProductId = purchaseOrderItem.ProductId.Value
                                };
                                context.ProductInventoryBalance.Add(inventoryBalance);
                                inventoryBalancesLocal.Add(inventoryBalance);
                            }
                        }
                        inventoryBalance.Quantity += purchaseOrderItem.Quantity;
                        context.ProductInventoryHistory.Add(new Dal.Models.ProductInventoryHistory
                        {
                            LocationId = purchaseOrderItem.LocationId.Value,
                            ProductId = purchaseOrderItem.ProductId.Value,
                            Quantity = purchaseOrderItem.Quantity,
                            Created = CurrentCambodiaTime,
                            Date = purchaseOrder.Date,
                            CreatedBy = purchaseOrder.CreatedBy,
                            Description = "Purchase " + purchaseOrder.OrderNumber,
                            RefNo = purchaseOrder.OrderNumber,
                            UnitPrice = purchaseOrderItem.UnitPrice * purchaseOrderDb.IdNavigation.BaseCurrencyExchangeRate.Value,
                            ProductInventoryHistoryIn = new Dal.Models.ProductInventoryHistoryIn
                            {

                            }
                        });
                    }

                }
            }
            context.ExpenseAttachentFile.RemoveRange(purchaseOrderDb.IdNavigation.ExpenseAttachentFile);
            foreach(var file in purchaseOrder.Attachments)
            {
                purchaseOrderDb.IdNavigation.ExpenseAttachentFile.Add(new Dal.Models.ExpenseAttachentFile
                {
                     FileName=file.FileName,
                     FileUrl=file.FileUrl,
                     IsFinalOfficalFile=file.IsFinalOfficalFile
                });
            }

            context.SaveChanges();
        }
        public List<StatusOverview> GetStatusOverviews(string organisationId)
        {
            using var context = Context();
            var khmer = IsKhmer;
            var organisationCurrencySymbole = context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId).BaseCurrency.Symbole;
            var bills = context.PurchaseOrder.Where(u => u.IdNavigation.OrganisationId == organisationId && (u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Draft || u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.WaitingForApproval || u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Approved))
                .Select(u => new { u.IdNavigation.ExpenseStatusId, u.IdNavigation.BaseCurrencyExchangeRate, u.IdNavigation.TotalIncludeTax })
                .GroupBy(u => u.ExpenseStatusId)
                       .Select(u => new
                       {
                           StatusId = u.Key,
                           Total = u.Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate),
                           Count = u.Count()
                       }).ToList();
            var status = (from st in context.ExpenseStatus.Where(u => u.Id == (int)ExpenseStatusEnum.Draft || u.Id == (int)ExpenseStatusEnum.WaitingForApproval || u.Id == (int)ExpenseStatusEnum.Approved)
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
            return status;
        }
    }
}
