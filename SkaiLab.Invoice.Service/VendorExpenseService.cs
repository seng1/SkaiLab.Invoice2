using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class VendorExpenseService:ExpenseService,IVendorExpenseService
    {
        public VendorExpenseService(IDataContext context) : base(context)
        {

        }

        public void Create(Expense expense)
        {
            using var context = Context();
            if (expense.VendorId == 0)
            {
                throw new Exception("Vendor is require");
            }
            if (expense.CurrencyId == 0)
            {
                throw new Exception("Currency is require");
            }
            if (expense.ExpenseItems == null || !expense.ExpenseItems.Any())
            {
                throw new Exception("Bill item is require");
            }

            if (expense.ExpenseItems.Any(u => u.Quantity < 0))
            {
                throw new Exception("Bill item quantity must >0");
            }
            if (expense.ExpenseItems.Any(u => u.UnitPrice < 0))
            {
                throw new Exception("Bill order item unit price must >=0");
            }
            var newOrder = new Dal.Models.Expense
            {
                ApprovedBy = "",
                ApprovedDate = null,
                BilledBy = "",
                BilledDate = null,
                Created = CurrentCambodiaTime,
                CreatedBy = expense.CreatedBy,
                CurrencyId = expense.CurrencyId,
                DeliveryDate = expense.DeliveryDate,
                Date = expense.Date,
                Note = expense.Note,
                Number = GetExpenseNumber(context, expense.OrganisationId),
                OrganisationId = expense.OrganisationId,
                ExpenseStatusId = expense.ExpenseStatusId,
                RefNo = expense.RefNo,
                VendorId = expense.VendorId,
                Total = 0,
                TotalIncludeTax = 0,
                VendorExpense = new Dal.Models.VendorExpense()

            };
            if (expense.Attachments != null && expense.Attachments.Any())
            {
                foreach (var attachment in expense.Attachments)
                {
                    newOrder.ExpenseAttachentFile.Add(new Dal.Models.ExpenseAttachentFile
                    {
                        FileUrl = attachment.FileUrl,
                        FileName=attachment.FileName,
                        IsFinalOfficalFile=attachment.IsFinalOfficalFile
                    });
                }
            }
            var taxCurrency = GetTaxCurrency(context, expense.OrganisationId);
            var baseCurrencyId = GetOrganisationBaseCurrencyId(context, expense.OrganisationId);
            if (baseCurrencyId == expense.CurrencyId)
            {
                newOrder.BaseCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = expense.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == baseCurrencyId);
                newOrder.BaseCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            if (taxCurrency.Id == expense.CurrencyId)
            {
                newOrder.TaxCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = expense.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == taxCurrency.Id);
                newOrder.TaxCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            foreach (var orderItem in expense.ExpenseItems)
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
                    LineTotalIncludeTax = lineTotalIncludeTax,
                    ExpenseProductItem = orderItem.ProductId == null ? null : new Dal.Models.ExpenseProductItem
                    {
                        ProductId = orderItem.ProductId.Value,
                        LocationId = orderItem.LocationId,
                    },
                    Description = orderItem.Description
                });
                newOrder.Total += lineTotal;
                newOrder.TotalIncludeTax += lineTotalIncludeTax;
            }
            context.Expense.Add(newOrder);
            context.SaveChanges();
            expense.Id = newOrder.Id;
            expense.OrderNumber = newOrder.Number;
        }

        public ExpenseForUpdate GetExpenseForUpdate(string organisationId, long id)
        {
            using var context = Context();
            var order = context.VendorExpense.FirstOrDefault(u => u.Id == id && u.IdNavigation.OrganisationId == organisationId);
            if (order == null)
            {
                throw new Exception("Expense not found");
            }
            var locations = context.Location.Where(u => u.OrganisationId == organisationId)
                .OrderBy(u => u.Name)
                .Select(u => new Location
                {
                    Name = u.Name,
                    Id = u.Id
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
                ExchangeRates = new List<CurrencyExchangeRate>()
            };
            currencies.Add(orderCurrency);
            if (order.IdNavigation.CurrencyId != baseCurrency.Id)
            {
                orderCurrency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                    ExchangeRate = order.IdNavigation.BaseCurrencyExchangeRate.Value,
                    CurrencyId = baseCurrency.Id,
                    Currency = baseCurrency
                });
            }
            if (order.IdNavigation.CurrencyId != taxCurrency.Id)
            {
                orderCurrency.ExchangeRates.Add(new CurrencyExchangeRate
                {
                    ExchangeRate = order.IdNavigation.TaxCurrencyExchangeRate.Value,
                    CurrencyId = taxCurrency.Id,
                    Currency = taxCurrency
                });
            }
            var taxes = GetTaxesIncludeComponent(context, organisationId);
            var purchaseOrder = new Expense
            {
                BaseCurrencyExchangeRate = order.IdNavigation.BaseCurrencyExchangeRate.Value,
                Currency = currencies.FirstOrDefault(u => u.Id == order.IdNavigation.CurrencyId),
                CurrencyId = order.IdNavigation.CurrencyId,
                OrderNumber = order.IdNavigation.Number,
                Id = order.Id,
                Date = order.IdNavigation.Date,
                DeliveryDate = order.IdNavigation.DeliveryDate,
                Note = order.IdNavigation.Note,
                RefNo = order.IdNavigation.RefNo,
                VendorId = order.IdNavigation.VendorId,
                Total = order.IdNavigation.Total,
                TotalIncludeTax = order.IdNavigation.TotalIncludeTax,
                TaxCurrencyExchangeRate = order.IdNavigation.TaxCurrencyExchangeRate.Value,
                ExpenseStatusId = order.IdNavigation.ExpenseStatusId,
                ExpenseStatus = new ExpenseStatus
                {
                    Id = order.IdNavigation.ExpenseStatusId,
                    Name = order.IdNavigation.ExpenseStatus.Name
                },
                OrganisationId = order.IdNavigation.OrganisationId,
                BilledBy = order.IdNavigation.BilledBy,
                ApprovedBy = order.IdNavigation.ApprovedBy,
                ApprovedDate = order.IdNavigation.ApprovedDate,
                BilledDate = order.IdNavigation.BilledDate,
                Created = order.IdNavigation.Created,
                CreatedBy = order.IdNavigation.CreatedBy,
                Attachments = order.IdNavigation.ExpenseAttachentFile.Select(u => new Attachment { IsFinalOfficalFile=u.IsFinalOfficalFile.Value,FileName=u.FileName,FileUrl=u.FileUrl}).ToList(),
                ExpenseItems = order.IdNavigation.ExpenseItem.ToList().Select(u => new PurchaseOrderItem
                {
                    DiscountRate = u.DiscountRate,
                    Id = u.Id,
                    LineTotal = u.LineTotal,
                    LineTotalIncludeTax = u.LineTotalIncludeTax,
                    ProductId = u.ExpenseProductItem?.ProductId,
                    UnitPrice = u.UnitPrice,
                    Quantity = u.Quantity,
                    TaxId = u.TaxId,
                    Tax = u.TaxId == null ? null : taxes.FirstOrDefault(t => t.Id == u.TaxId),
                    Description = u.Description

                }).ToList(),
                Vendor = new Vendor
                {
                    Id = order.IdNavigation.VendorId,
                    DisplayName = order.IdNavigation.Vendor.DisplayName
                }
            };
            return new ExpenseForUpdate
            {
                Currencies = currencies,
                Locations = locations,
                Taxes = taxes,
                Expense = purchaseOrder,
                BaseCurrencyId = baseCurrency.Id,
                TaxCurrency = taxCurrency
            };
        }

        public string GetExpenseNumber(string organisationId)
        {
            return GetExpenseNumber(Context(), organisationId);
        }
        public string GetExpenseNumber(Dal.Models.InvoiceContext context, string organisationId)
        {
            var orderNum = context.VendorExpense.Where(u => u.IdNavigation.OrganisationId == organisationId).Count() + 1;
            return "EP-" + orderNum.ToString("000000");
        }
        public Expense GetPurchase(string organisationId, long id)
        {
            throw new NotImplementedException();
        }
        public List<ExpenseStatus> GetExpenseStatuses(PurchaseOrderFilter filter)
        {
            var context = Context();
            var statuses = context.VendorExpense.Where(u => u.IdNavigation.OrganisationId == filter.OrganisationId &&
                                                            u.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.Delete &&
                                                           (filter.VendorId == 0 || u.IdNavigation.VendorId == filter.VendorId) &&
                                                           (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate) &&
                                                           (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate) &&
                                                           (string.IsNullOrEmpty(filter.SearchText) || u.IdNavigation.Number.Contains(filter.SearchText))
                                                         )
                            .GroupBy(u => new { u.IdNavigation.ExpenseStatusId })
                            .Select(u => new
                            {
                                Id = u.Key.ExpenseStatusId,
                                Count = u.Count()
                            }).ToList();
            var result = context.ExpenseStatus.Where(u => u.Id != (int)ExpenseStatusEnum.Delete).ToList()
                .Select(u => new ExpenseStatus
                {
                    Id = u.Id,
                    Name = u.Name,
                    Count = statuses.Any(c => c.Id == u.Id) ? statuses.FirstOrDefault(c => c.Id == u.Id).Count : 0
                }).ToList();
            result.Insert(0, new ExpenseStatus
            {
                Id = 0,
                Name = "All",
                Count = statuses.Sum(u => u.Count)
            });
            return result;
        }
        public List<Expense> GetExpenses(PurchaseOrderFilter filter)
        {
            var context = Context();
            var orders = context.VendorExpense.Where(u => u.IdNavigation.OrganisationId == filter.OrganisationId &&
                                                          u.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.Delete &&
                                                        (filter.PurchaseOrderStatusId == 0 || u.IdNavigation.ExpenseStatusId == filter.PurchaseOrderStatusId) &&
                                                        (filter.VendorId == 0 || u.IdNavigation.VendorId == filter.VendorId) &&
                                                        (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate) &&
                                                        (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate) &&
                                                        (string.IsNullOrEmpty(filter.SearchText) || u.IdNavigation.Number.Contains(filter.SearchText))
                                                       ).OrderByDescending(u => u.IdNavigation.Number)
                                                       .Skip((filter.Page - 1) * filter.PageSize)
                                                       .Take(filter.PageSize)
                                                       .Select(u => new Expense
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
                                                               Name = u.IdNavigation.ExpenseStatus.Name,
                                                           },
                                                           ExpenseStatusId = u.IdNavigation.ExpenseStatusId,
                                                           RefNo = u.IdNavigation.RefNo,
                                                           TaxCurrencyExchangeRate = u.IdNavigation.TaxCurrencyExchangeRate.Value,
                                                           Total = u.IdNavigation.Total,
                                                           TotalIncludeTax = u.IdNavigation.TotalIncludeTax,
                                                           VendorId = u.IdNavigation.VendorId,
                                                           Vendor = new Vendor
                                                           {
                                                               Id = u.IdNavigation.VendorId,
                                                               DisplayName = u.IdNavigation.Vendor.DisplayName
                                                           }
                                                       }).ToList();
            return orders;
        }

        public void GetTotalPages(PurchaseOrderFilter filter)
        {
            var context = Context();
            filter.TotalRow = context.VendorExpense.Where(u => u.IdNavigation.OrganisationId == filter.OrganisationId &&
                                                            u.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.Delete &&
                                                           (filter.PurchaseOrderStatusId == 0 || u.IdNavigation.ExpenseStatusId == filter.PurchaseOrderStatusId) &&
                                                           (filter.VendorId == 0 || u.IdNavigation.VendorId == filter.VendorId) &&
                                                           (filter.FromDate == null || u.IdNavigation.Date >= filter.FromDate) &&
                                                           (filter.ToDate == null || u.IdNavigation.Date <= filter.ToDate) &&
                                                           (string.IsNullOrEmpty(filter.SearchText) || u.IdNavigation.Number.Contains(filter.SearchText))
                                                         ).Count();
        }

        public void MarkAsApprove(List<long> expenseIds, string organisationId, string approvedBy)
        {
            using var context = Context();
            var billes = context.VendorExpense.Where(u => u.IdNavigation.OrganisationId == organisationId
                                  && u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.WaitingForApproval &&
                                  expenseIds.Any(t => t == u.Id))
                                 .Select(u => u).ToList();
            if (expenseIds.Count < expenseIds.Count)
            {
                throw new Exception("We cannot move expense to approved because some expense already move status.");
            }
            foreach (var purchaseOrder in billes)
            {
                purchaseOrder.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Approved;
                purchaseOrder.IdNavigation.ApprovedBy = approvedBy;
                purchaseOrder.IdNavigation.ApprovedDate = CurrentCambodiaTime;

            }
            context.SaveChanges();
        }

        public void MarkAsBill(Expense expense)
        {
            using var context = Context();
            var bil = context.VendorExpense.FirstOrDefault(u => u.Id == expense.Id && u.IdNavigation.OrganisationId == expense.OrganisationId);
            if (bil == null)
            {
                throw new Exception("Expense not found");
            }
            if (bil.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.Approved)
            {
                throw new Exception("Expense status does not allow to mark as bill");
            }
            bil.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Billed;
            bil.IdNavigation.BilledBy = expense.BilledBy;
            bil.IdNavigation.BilledDate = CurrentCambodiaTime;
            context.SaveChanges();
        }

        public void MarkAsBill(List<long> expenseIds, string organisationId, string billBy)
        {
            using var context = Context();
            var bills = context.VendorExpense.Where(u => u.IdNavigation.OrganisationId == organisationId
                                  && u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Approved &&
                                  expenseIds.Any(t => t == u.Id))
                                 .Select(u => u).ToList();
            if (expenseIds.Count < bills.Count)
            {
                throw new Exception("We cannot move expense to bill status because some expense already move status.");
            }
            foreach (var purchaseOrder in bills)
            {
                purchaseOrder.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Billed;
                purchaseOrder.IdNavigation.BilledBy = billBy;
                purchaseOrder.IdNavigation.BilledDate = CurrentCambodiaTime;
            }
            context.SaveChanges();
        }

        public void MarkAsDelete(List<long> expenseIds, string organisationId, string deletBy)
        {
            using var context = Context();
            var bills = context.VendorExpense.Where(u => u.IdNavigation.OrganisationId == organisationId
                                  && (u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Draft ||
                                  u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.WaitingForApproval) &&
                                  expenseIds.Any(t => t == u.Id))
                                 .Select(u => u).ToList();
            if (expenseIds.Count < bills.Count)
            {
                throw new Exception("We cannot delete expense  because some expense already move status.");
            }
            foreach (var purchaseOrder in bills)
            {
                purchaseOrder.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Delete;
                purchaseOrder.IdNavigation.DeletedBy = deletBy;
                purchaseOrder.IdNavigation.DeletedDate = CurrentCambodiaTime;
            }
            context.SaveChanges();
        }

        public void MarkAsWaitingForApproval(List<long> expenseIds, string organisationId)
        {
            using var context = Context();
            var bills = context.VendorExpense.Where(u => u.IdNavigation.OrganisationId == organisationId
                                  && u.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Draft &&
                                  expenseIds.Any(t => t == u.Id))
                                 .Select(u => u).ToList();
            if (bills.Count < expenseIds.Count)
            {
                throw new Exception("We cannot move exepnse to waiting for approval because some expense already move status.");
            }
            foreach (var purchaseOrder in bills)
            {
                purchaseOrder.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.WaitingForApproval;
            }
            context.SaveChanges();
        }

        public void MarkDelete(Expense expense)
        {
            using var context = Context();
            var bill = context.VendorExpense.FirstOrDefault(u => u.Id == expense.Id && u.IdNavigation.OrganisationId == expense.OrganisationId);
            if (bill == null)
            {
                throw new Exception("Expense not found");
            }
            if (bill.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.WaitingForApproval
                && bill.IdNavigation.ExpenseStatusId != (int)ExpenseStatusEnum.Draft)
            {
                throw new Exception("Expense doesn't allow to delete");
            }
            bill.IdNavigation.ExpenseStatusId = (int)ExpenseStatusEnum.Delete;
            bill.IdNavigation.DeletedBy = expense.DeletedBy;
            bill.IdNavigation.DeletedDate = CurrentCambodiaTime;
            context.SaveChanges();
        }

        public void Update(Expense purchaseOrder)
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
                throw new Exception("Expense item is require");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.ProductId == 0))
            {
                throw new Exception("Product is require");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.Quantity < 0))
            {
                throw new Exception("Expense item quantity must >0");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.UnitPrice < 0))
            {
                throw new Exception("Expense item unit price must >=0");
            }
            if (purchaseOrder.ExpenseItems.Any(u => u.DiscountRate != null && (u.DiscountRate < 0 || u.DiscountRate > 100)))
            {
                throw new Exception("Expense item discount rate must between 0 and 100");
            }
            var expense = context.VendorExpense.FirstOrDefault(u => u.Id == purchaseOrder.Id && u.IdNavigation.OrganisationId == purchaseOrder.OrganisationId);
            if (expense == null)
            {
                throw new Exception("No expense for update");
            }
            if (expense.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Approved
                || expense.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Billed
                || expense.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Delete)
            {
                throw new Exception("Expense status doesn't allow to update");
            }
            expense.IdNavigation.VendorId = purchaseOrder.VendorId;
            expense.IdNavigation.Date = purchaseOrder.Date;
            expense.IdNavigation.DeliveryDate = purchaseOrder.DeliveryDate;
            expense.IdNavigation.RefNo = purchaseOrder.RefNo;
            expense.IdNavigation.CurrencyId = purchaseOrder.CurrencyId;
            expense.IdNavigation.Note = purchaseOrder.Note;
            expense.IdNavigation.ExpenseStatusId = purchaseOrder.ExpenseStatusId;
            var taxCurrency = GetTaxCurrency(context, purchaseOrder.OrganisationId);
            var baseCurrencyId = GetOrganisationBaseCurrencyId(context, purchaseOrder.OrganisationId);
            if (baseCurrencyId == purchaseOrder.CurrencyId)
            {
                expense.IdNavigation.BaseCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = purchaseOrder.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == baseCurrencyId);
                expense.IdNavigation.BaseCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            if (taxCurrency.Id == purchaseOrder.CurrencyId)
            {
                expense.IdNavigation.TaxCurrencyExchangeRate = 1;
            }
            else
            {
                var exchangeRate = purchaseOrder.Currency.ExchangeRates.FirstOrDefault(u => u.CurrencyId == taxCurrency.Id);
                expense.IdNavigation.TaxCurrencyExchangeRate = exchangeRate.ExchangeRate;
            }
            expense.IdNavigation.Total = 0;
            expense.IdNavigation.TotalIncludeTax = 0;
            var orderItemsIds = purchaseOrder.ExpenseItems.Where(u => u.Id != 0).Select(u => u.Id).ToList();
            var removeOrderItems = context.ExpenseItem.Where(u => u.ExpenseId == purchaseOrder.Id && !orderItemsIds.Any(t => u.Id == t));
            if (removeOrderItems.Any())
            {
                var removeProducts = removeOrderItems.Where(u => u.ExpenseProductItem != null).Select(u => u.ExpenseProductItem);
                if (removeProducts.Any())
                {
                    context.ExpenseProductItem.RemoveRange(removeProducts);
                }
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
                    expense.IdNavigation.ExpenseItem.Add(new Dal.Models.ExpenseItem
                    {
                        DiscountRate = orderItem.DiscountRate,
                        Quantity = orderItem.Quantity,
                        TaxId = orderItem.TaxId,
                        UnitPrice = orderItem.UnitPrice,
                        LineTotal = lineTotal,
                        LineTotalIncludeTax = lineTotalIncludeTax,
                        Description = orderItem.Description,
                        ExpenseProductItem = orderItem.ProductId == null ? null : new Dal.Models.ExpenseProductItem
                        {
                            ProductId = orderItem.ProductId.Value,
                            LocationId = orderItem.LocationId
                        }
                    });
                }
                else
                {
                    var orderItemdb = context.ExpenseItem.FirstOrDefault(u => u.Id == orderItem.Id);
                    orderItemdb.DiscountRate = orderItem.DiscountRate;
                    orderItemdb.TaxId = orderItem.TaxId;
                    orderItemdb.Quantity = orderItem.Quantity;
                    orderItemdb.LineTotal = lineTotal;
                    orderItemdb.LineTotalIncludeTax = lineTotalIncludeTax;
                    if (orderItem.ProductId != null)
                    {
                        if (orderItemdb.ExpenseProductItem == null)
                        {
                            orderItemdb.ExpenseProductItem = new Dal.Models.ExpenseProductItem();
                        }
                        orderItemdb.ExpenseProductItem.ProductId = orderItem.ProductId.Value;
                    }
                    else if (orderItemdb.ExpenseProductItem != null)
                    {
                        context.ExpenseProductItem.Remove(orderItemdb.ExpenseProductItem);
                    }

                }
                expense.IdNavigation.Total += lineTotal;
                expense.IdNavigation.TotalIncludeTax += lineTotalIncludeTax;
            }
            context.SaveChanges();
        }
    }
}
