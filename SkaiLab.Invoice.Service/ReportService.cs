using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class ReportService:Service,IReportService
    {
        public ReportService(IDataContext context) : base(context)
        {

        }

        public CustomerBalanceDetail GetCustomerBalanceDetail(ReportFilter filter)
        {
            var organisationIds = filter.OrganisationIds;
            using var context = Context();
            var result = (from invoice in context.Invoice
                          where organisationIds.Any(t=>t==invoice.IdNavigation.OrganisationId) &&
                          invoice.StatusId == (int)InvoiceStatusEnum.WaitingForPayment
                          select new CustomerBalanceDetailItem
                          {
                              Amount = invoice.IdNavigation.TotalIncludeTax * invoice.IdNavigation.BaseCurrencyExchangeRate,
                              Date = invoice.IdNavigation.Date,
                              DueDate = invoice.IdNavigation.DueDate,
                              Number = invoice.IdNavigation.Number,
                              TransactionType = "Invoice",
                              ParentId=invoice.Id,
                              PaidDate=invoice.IdNavigation.PaidDate,
                              Customer=new Customer
                              {
                                  Id=invoice.IdNavigation.CustomerId,
                                  DisplayName=invoice.IdNavigation.Customer.DisplayName
                              }
                          }).ToList();
            return new CustomerBalanceDetail
            {
                CustomerBalanceDetailHeaders = (from r in result
                                               group  r by new { r.Customer.Id,r.Customer.DisplayName} into g
                                               select new CustomerBalanceDetailHeader
                                               {
                                                   Customer=new Customer
                                                   {
                                                       Id=g.Key.Id,
                                                       DisplayName=g.Key.DisplayName
                                                   },
                                                   IsExpand=true,
                                                   CustomerBalanceDetailItems=g.Select(u=>u).ToList()
                                               }).ToList()
                
            };
        }

        public List<CustomerBalanceSummary> GetCustomerBalanceSummary(ReportFilter filter)
        {
            var organisationIds = filter.OrganisationIds;
            using var context = Context();
            var result = (from invoice in context.Invoice
                          where organisationIds.Any(t => t == invoice.IdNavigation.OrganisationId) &&
                          invoice.StatusId == (int)InvoiceStatusEnum.WaitingForPayment
                          select new { invoice.IdNavigation.CustomerId, invoice.IdNavigation.Customer.DisplayName, Total=invoice.IdNavigation.TotalIncludeTax*invoice.IdNavigation.BaseCurrencyExchangeRate }
                          ).GroupBy(u => new { u.CustomerId, u.DisplayName })
                          .Select(u => new CustomerBalanceSummary
                          {
                              Customer=new Customer
                              {
                                  Id=u.Key.CustomerId,
                                  DisplayName=u.Key.DisplayName,
                              },
                              Total=u.Sum(u=>u.Total )
                          }).ToList();
            return result;
        }

        public List<Models.Invoice> GetCustomerInvoices(ReportFilter filter)
        {
            using var context = Context();
            var organisationIds = filter.OrganisationIds;
            var invoices = (from invoice in context.Invoice
                            where organisationIds.Any(t => t == invoice.IdNavigation.OrganisationId) &&
                            invoice.IdNavigation.Date >= filter.FromDate && invoice.IdNavigation.Date <= filter.ToDate
                            select new Models.Invoice
                            {
                                Id=invoice.Id,
                                Number=invoice.IdNavigation.Number,
                                RefNo=invoice.IdNavigation.RefNo,
                                Customer=new Customer
                                {
                                    Id=invoice.IdNavigation.CustomerId,
                                    DisplayName=invoice.IdNavigation.Customer.DisplayName,
                                },
                                Date=invoice.IdNavigation.Date,
                                DueDate=invoice.IdNavigation.DueDate,
                                PaidDate=invoice.IdNavigation.PaidDate,
                                TotalIncludeTax=invoice.IdNavigation.TotalIncludeTax * invoice.IdNavigation.BaseCurrencyExchangeRate
                            }).ToList();
            return invoices;

        }

        public List<ExpenseItem> GetExpenseItemsTax(List<long> expenseIds)
        {
            using var context = Context();
            var expenseItems = (from ex in context.ExpenseItem
                                where expenseIds.Any(t => t == ex.ExpenseId) &&
                                ex.TaxId != null
                                select new ExpenseItem
                                {
                                    UnitPrice=ex.UnitPrice,
                                    TaxId=ex.TaxId,
                                    Description=ex.Description,
                                    DiscountRate=ex.DiscountRate,
                                    Id=ex.Id,
                                    LineTotal=ex.LineTotal,
                                    LineTotalIncludeTax=ex.LineTotalIncludeTax,
                                    Quantity=ex.Quantity,
                                    Tax=new Tax
                                    {
                                        Id=ex.TaxId.Value,
                                        Name=ex.Tax.Name,
                                        Components=ex.Tax.TaxComponent.Select(u=>new TaxComponent
                                        {
                                            Id=u.Id,
                                            Name=u.Name,
                                            Rate=u.Rate
                                        }).ToList()
                                    },
                                    ExpenseId=ex.ExpenseId
                                }).ToList();
            return expenseItems;

        }

        public List<Attachment> GetExpenseOfficalDocuments(List<long> expenseIds)
        {
            using var context = Context();
            var attachments = (from a in context.ExpenseAttachentFile
                               where a.IsFinalOfficalFile == true && expenseIds.Any(t => t == a.ExpenseId)
                               select new Attachment
                               {
                                   FileName=a.FileName,
                                   FileUrl=a.FileUrl,
                                   IsFinalOfficalFile=true,
                                   RefNo=string.IsNullOrEmpty(a.Expense.RefNo)?a.Expense.Number:a.Expense.RefNo
                               }).ToList();
            return attachments;

        }

        public List<InventoryHistoryDetail> GetInventoryHistories(string organisationId, long productId, InventoryHistoryFilter reportFilter)
        {
            using var context = Context();
            var inventories = context.ProductInventoryHistory.Where(u => u.ProductId == productId && u.Product.IdNavigation.OrganisationId == organisationId && u.Date >= reportFilter.FromDate && u.Date <= reportFilter.ToDate)
                .OrderByDescending(u => u.Date)
                .Skip((reportFilter.Page - 1) * reportFilter.PageSize)
                .Take(reportFilter.PageSize)
                .Select(u => new InventoryHistoryDetail
                {
                    Location=new Location
                    {
                        Id=u.LocationId,
                        Name=u.Location.Name,
                    },
                    LocationId=u.LocationId,
                    RefNo=u.RefNo,
                    UnitPrice=u.UnitPrice,
                    Date=u.Date,
                    Quantity=u.ProductInventoryHistoryOut==null?u.Quantity:u.Quantity*-1
                    
                }).ToList();
            return inventories;
        }

        public List<CustomerTransactionItem> GetInvoiceItems(List<long> invoiceIds)
        {
            using var context = Context();
            var invoiceItems = (from ex in context.CustomerTransactionItem
                                where invoiceIds.Any(t => t == ex.CustomerTransactionId) &&
                                ex.TaxId != null
                                select new CustomerTransactionItem
                                {
                                    UnitPrice = ex.UnitPrice,
                                    TaxId = ex.TaxId,
                                    Description = ex.Description,
                                    DiscountRate = ex.DiscountRate,
                                    Id = ex.Id,
                                    LineTotal = ex.LineTotal,
                                    LineTotalIncludeTax = ex.LineTotalIncludeTax,
                                    Quantity = ex.Quantity,
                                    Tax = new Tax
                                    {
                                        Id = ex.TaxId.Value,
                                        Name = ex.Tax.Name,
                                        Components = ex.Tax.TaxComponent.Select(u => new TaxComponent
                                        {
                                            Id = u.Id,
                                            Name = u.Name,
                                            Rate = u.Rate
                                        }).ToList()
                                    },
                                    CustomerTransaction = ex.CustomerTransactionId
                                }).ToList();
            return invoiceItems;
        }

        public List<Attachment> GetInvoiceOfficalDocuments(List<long> invoiceIds)
        {
            using var context = Context();
            var attachments = (from a in context.CustomerTransactionAttachment
                               where a.IsFinalOfficalFile == true && invoiceIds.Any(t => t == a.CustomerTransactionId)
                               select new Attachment
                               {
                                   FileName = a.FileName,
                                   FileUrl = a.FileUrl,
                                   IsFinalOfficalFile = true,
                                   RefNo = a.CustomerTransaction.Number
                               }).ToList();
            return attachments;
        }

        public List<Product> GetProductInventoriesBalance(string organisationId, string searchText)
        {
            using var context = Context();
            var products = context.Product.Where(u => u.ProductInventory != null && u.OrganisationId==organisationId && (string.IsNullOrEmpty(searchText)||u.Code.Contains(searchText)||u.Name.Contains(searchText)))
                .Select(u => new Product
                {
                    Code = u.Code,
                    Id = u.Id,
                    ImageUrl = u.ImageUrl,
                    Name = u.Name,
                    ProductPurchaseInformation = new ProductSalePurchaseDetail
                    {
                        Price = u.ProductPurchaseInformation.Price,
                        Title = u.ProductPurchaseInformation.Title,
                        Description = u.ProductPurchaseInformation.Description,
                    },
                    ProductSaleInformation = new ProductSalePurchaseDetail
                    {
                        Description = u.ProductSaleInformation.Description,
                        Title = u.ProductSaleInformation.Title,
                        Price = u.ProductSaleInformation.Price
                    },
                }).ToList();
            var inventoryBalace = (from balance in context.ProductInventoryBalance
                                   group balance by balance.ProductId into g
                                   select new
                                   {
                                       ProductId = g.Key,
                                       Balance = g.Sum(u => u.Quantity)
                                   }).ToList();
            var result = (from p in products
                          join b in inventoryBalace on p.Id equals b.ProductId into pb
                          from subB in pb.DefaultIfEmpty()
                          select new Product
                          {
                              Id = p.Id,
                              Code = p.Code,
                              Name = p.Name,
                              ImageUrl = p.ImageUrl,
                              ProductPurchaseInformation = p.ProductPurchaseInformation,
                              ProductSaleInformation = p.ProductSaleInformation,
                              QtyBalance = subB == null ? 0 : subB.Balance
                          }).ToList();
            return result;
        }

        public List<Product> GetProductInventoryByLocation(string organisationId, long productId)
        {
            using var context = Context();
            var inventoryBalances = context.ProductInventoryBalance.Where(u => u.ProductId == productId && u.Product.IdNavigation.OrganisationId == organisationId)
                .Select(u => new
                {
                    u.LocationId,
                    u.Quantity
                }).ToList();
            var locations = (from location in context.Location.Where(u => u.OrganisationId == organisationId).Select(u => new { u.Id, u.Name }).ToList()
                             join b in inventoryBalances on location.Id equals b.LocationId into lb
                             from subLb in lb.DefaultIfEmpty()
                             select new Product
                             {
                                 Location=new Location
                                 {
                                     Id=location.Id,
                                     Name=location.Name,

                                 },
                                 QtyBalance=subLb==null?0:subLb.Quantity
                             }).ToList();
            return locations;
        }

        public ProductSaleDetail GetProductSaleDetail(long productId, ReportFilter reportFilter)
        {
            using var context = Context();
            var organisationIds = reportFilter.OrganisationIds;
            var purchases = (from p in context.ExpenseProductItem
                            where p.IdNavigation.Expense.Date >= reportFilter.FromDate && p.IdNavigation.Expense.Date <= reportFilter.ToDate &&
                            p.ProductId == productId &&
                            organisationIds.Any(t => t == p.IdNavigation.Expense.OrganisationId) &&
                           (p.IdNavigation.Expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || p.IdNavigation.Expense.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                           orderby p.IdNavigation.Expense.Date descending
                            select new ProductSaleDetailItem
                            {
                                Date = p.IdNavigation.Expense.Date,
                                DiscountRate = p.IdNavigation.DiscountRate,
                                Qty = p.IdNavigation.Quantity,
                                TaxRate = p.IdNavigation.Tax == null ? (decimal?) null : p.IdNavigation.Tax.TaxComponent.Sum(u => u.Rate),
                                To=p.IdNavigation.Expense.Vendor.DisplayName,
                                UnitPrice=p.IdNavigation.UnitPrice,
                                RefNo=p.IdNavigation.Expense.Number
                            }).ToList();


            var sales = (from p in context.CustomerTransactionItemProduct
                         where p.IdNavigation.CustomerTransaction.Date >= reportFilter.FromDate && p.IdNavigation.CustomerTransaction.Date <= reportFilter.ToDate &&
                         p.ProductId == productId &&
                         p.IdNavigation.CustomerTransaction.Invoice != null &&
                         organisationIds.Any(t => t == p.IdNavigation.CustomerTransaction.OrganisationId)
                         orderby p.IdNavigation.CustomerTransaction.Date descending
                         select new ProductSaleDetailItem
                         {
                             Date = p.IdNavigation.CustomerTransaction.Date,
                             DiscountRate = p.IdNavigation.DiscountRate,
                             Qty = p.IdNavigation.Quantity,
                             TaxRate = p.IdNavigation.Tax == null ? (decimal?)null : p.IdNavigation.Tax.TaxComponent.Sum(u => u.Rate),
                             To = p.IdNavigation.CustomerTransaction.Customer.DisplayName,
                             UnitPrice = p.IdNavigation.UnitPrice,
                             RefNo = p.IdNavigation.CustomerTransaction.Number
                         }).ToList();
            return new ProductSaleDetail
            {
                PurchaseItems = purchases,
                SaleItems = sales
            };
        }

        public List<ProductSaleSummary> GetProductSaleSummaries(ReportFilter reportFilter)
        {
            using var context = Context();
            var organisationIds = reportFilter.OrganisationIds;
            var purchases = (from p in context.ExpenseProductItem
                             where p.IdNavigation.Expense.CloseDate >= reportFilter.FromDate && p.IdNavigation.Expense.CloseDate <= reportFilter.ToDate &&
                             organisationIds.Any(t => t == p.IdNavigation.Expense.OrganisationId) &&
                            (p.IdNavigation.Expense.ExpenseStatusId==(int)ExpenseStatusEnum.Approved || p.IdNavigation.Expense.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                             select new { p.IdNavigation.Quantity, p.IdNavigation.LineTotalIncludeTax, p.ProductId }
                          ).GroupBy(u => u.ProductId)
                         .Select(u => new
                         {
                             ProductId=u.Key,
                             Qty=u.Sum(t=>t.Quantity),
                             total=u.Sum(t=>t.LineTotalIncludeTax)
                         }).ToList();
            var sales = (from p in context.CustomerTransactionItemProduct
                             where p.IdNavigation.CustomerTransaction.Date >= reportFilter.FromDate && p.IdNavigation.CustomerTransaction.Date <= reportFilter.ToDate &&
                             p.IdNavigation.CustomerTransaction.Invoice!=null &&
                             organisationIds.Any(t => t == p.IdNavigation.CustomerTransaction.OrganisationId)
                             select new { p.IdNavigation.Quantity, p.IdNavigation.LineTotalIncludeTax, p.ProductId }
                         ).GroupBy(u => u.ProductId)
                        .Select(u => new
                        {
                            ProductId = u.Key,
                            Qty = u.Sum(t => t.Quantity),
                            total = u.Sum(t => t.LineTotalIncludeTax)
                        }).ToList();
            var products = context.Product.Where(u=>organisationIds.Any(t=>t==u.OrganisationId)).Select(u => new Product
            {
                Id=u.Id,
                Code=u.Code,
                Name=u.Name
            }).ToList();
            var result = (from product in products
                          join purchase in purchases on product.Id equals purchase.ProductId into pp
                          from subPp in pp.DefaultIfEmpty()
                          join sale in sales on product.Id equals sale.ProductId into ps
                          from subPs in ps.DefaultIfEmpty()
                          select new ProductSaleSummary
                          {
                              Product=product,
                              AvgPurchasePrice=subPp==null?0:subPp.total/subPp.Qty,
                              PurchaseQty=subPp==null?0:subPp.Qty,
                              PurchaseTotal=subPp==null?0:subPp.total,
                              AvgSalePrice=subPs==null?0:subPs.total/subPs.Qty,
                              SaleQty=subPs==null?0:subPs.Qty,
                              SaleTotal=subPs==null?0:subPs.total,
                              NetQty=(subPp==null?0:subPp.Qty) - (subPs==null?0:subPs.Qty),
                              NetTotal = (subPs == null ? 0 : subPs.total)-(subPp == null ? 0 : subPp.total),
                          }).OrderByDescending(u=>u.SaleQty).ThenBy(u=>u.Product.Code).ToList();
            return result;

        }

        public ProfitAndLostDetail GetProfitAndLostDetail(ReportFilter filter)
        {
            var organisationIds = filter.OrganisationIds;
            using var context = Context();
            var invoice = AppResource.GetResource("Invoice");
            var invoiceItems = (from invoiceItem in context.CustomerTransactionItem
                                where invoiceItem.CustomerTransaction.Invoice != null &&
                                organisationIds.Any(t => t == invoiceItem.CustomerTransaction.OrganisationId) &&
                                invoiceItem.CustomerTransaction.Date >= filter.FromDate && invoiceItem.CustomerTransaction.Date <= filter.ToDate
                                select new ProfitAndLostDetailItem
                                {
                                    Amount=invoiceItem.LineTotalIncludeTax * invoiceItem.CustomerTransaction.BaseCurrencyExchangeRate,
                                    ClientOrVendorName=invoiceItem.CustomerTransaction.Customer.DisplayName,
                                    Date=invoiceItem.CustomerTransaction.Date,
                                    Description=invoiceItem.CustomerTransactionItemProduct==null?invoiceItem.Description:invoiceItem.CustomerTransactionItemProduct.Product.ProductSaleInformation.Title,
                                    Number=invoiceItem.CustomerTransaction.Number,
                                    TransactionType= invoice,
                                    ParentId=invoiceItem.CustomerTransactionId
                                    
                                }).ToList();
            var purchase = AppResource.GetResource("Purchase");
            var purchaseItems= (from expense in context.ExpenseItem
                                where expense.Expense.PurchaseOrder!=null &&
                                organisationIds.Any(t => t == expense.Expense.OrganisationId) &&
                                expense.Expense.Date >= filter.FromDate && expense.Expense.Date <= filter.ToDate &&
                                 (expense.Expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || expense.Expense.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                                select new ProfitAndLostDetailItem
                                {
                                    Amount = expense.LineTotalIncludeTax * expense.Expense.BaseCurrencyExchangeRate.Value,
                                    ClientOrVendorName = expense.Expense.Vendor.DisplayName,
                                    Date = expense.Expense.Date,
                                    Description = expense.ExpenseProductItem == null ? expense.Description : expense.ExpenseProductItem.Product.ProductPurchaseInformation.Title,
                                    Number = expense.Expense.Number,
                                    TransactionType = purchase,
                                    ParentId = expense.ExpenseId

                                }).ToList();
            var otherExpenseText = AppResource.GetResource("Other Expense");
            var otherExpense = (from expense in context.ExpenseItem
                                 where expense.Expense.PurchaseOrder == null &&
                                 organisationIds.Any(t => t == expense.Expense.OrganisationId) &&
                                 expense.Expense.Date >= filter.FromDate && expense.Expense.Date <= filter.ToDate &&
                                  (expense.Expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || expense.Expense.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                                 select new ProfitAndLostDetailItem
                                 {
                                     Amount = expense.LineTotalIncludeTax * expense.Expense.BaseCurrencyExchangeRate.Value,
                                     ClientOrVendorName = expense.Expense.Vendor.DisplayName,
                                     Date = expense.Expense.Date,
                                     Description = expense.ExpenseProductItem==null?expense.Description:expense.ExpenseProductItem.Product.ProductPurchaseInformation.Title,
                                     Number = expense.Expense.Number,
                                     TransactionType = otherExpenseText,
                                     ParentId = expense.ExpenseId

                                 }).ToList();
            var employeeSalaryText = AppResource.GetResource("Employee Salary");
            var employeeSalary = (from expense in context.PayrollEmployee
                                where 
                                organisationIds.Any(t => t == expense.PayrollMonth.OrganisationId) &&
                                expense.Date >= filter.FromDate && expense.Date <= filter.ToDate
                                select new ProfitAndLostDetailItem
                                {
                                    Amount =expense.Total,
                                    ClientOrVendorName = "",
                                    Date = expense.Date,
                                    Description = expense.Employee.DisplayName,
                                    Number =expense.PayrollMonth.Month,
                                    TransactionType = employeeSalaryText,
                                    ParentId = 0

                                }).ToList();
            var result = new ProfitAndLostDetail
            {
                ProfitAndLostDetailParents = new List<ProfitAndLostDetailParent>(),
                ProfitAndLostDetailTotal = new ProfitAndLostDetailTotal
                {
                    Name = AppResource.GetResource("Total"),
                    Total = invoiceItems.Sum(u => u.Amount) -( purchaseItems.Sum(u => u.Amount) + otherExpense.Sum(u => u.Amount) + employeeSalary.Sum(u=>u.Amount))
                }
            };
            result.ProfitAndLostDetailParents.Add(new ProfitAndLostDetailParent
            {
                Name = AppResource.GetResource("Invoice"),
                Total = invoiceItems.Sum(u => u.Amount),
                IsExpand = true,
                ProfitAndLostDetailItems = invoiceItems
            });
            result.ProfitAndLostDetailParents.Add(new ProfitAndLostDetailParent
            {
                Name = AppResource.GetResource("Purchase"),
                Total = purchaseItems.Sum(u => u.Amount),
                IsExpand = true,
                ProfitAndLostDetailItems = purchaseItems
            });
            result.ProfitAndLostDetailParents.Add(new ProfitAndLostDetailParent
            {
                Name = AppResource.GetResource("Other Expense"),
                Total = otherExpense.Sum(u => u.Amount),
                IsExpand = true,
                ProfitAndLostDetailItems = otherExpense
            });
            result.ProfitAndLostDetailParents.Add(new ProfitAndLostDetailParent
            {
                Name = AppResource.GetResource("Employee Salary"),
                Total = employeeSalary.Sum(u => u.Amount),
                IsExpand = true,
                ProfitAndLostDetailItems = employeeSalary
            });
            return result;
        }

        public ProfitAndLostSummary GetProfitAndLostSummary(ProfitAndLostSummaryFilter filter)
        {
            if (filter.DisplayColumn.Id == (int)DisplayColumnEnum.TotalOnly)
            {
                return GetProfitAndLostSummaryByTotalOnly(filter);
            }
            if (filter.DisplayColumn.Id == (int)DisplayColumnEnum.Month)
            {
                return GetProfitAndLostSummaryByMonth(filter);
            }
            return GetProfitAndLostSummaryByOrganisation(filter);
        }

        public TaxMonthly GetTaxMonthly(string month, string organisationId)
        {
            using var context = Context();
            var organisaction = context.Organisation.FirstOrDefault(u => u.Id == organisationId);
            if (!organisaction.DeclareTax)
            {
                throw new Exception("This organisaction does not delcare tax");
            }
            var result = new TaxMonthly
            {
                Currency=new Currency
                {
                    Code=organisaction.OrganisationBaseCurrency.BaseCurrency.Code,
                    Id=organisaction.OrganisationBaseCurrency.BaseCurrency.Id,
                    Name=organisaction.OrganisationBaseCurrency.BaseCurrency.Name,
                    Symbole=organisaction.OrganisationBaseCurrency.BaseCurrency.Symbole
                },
                TaxCurrency=new Currency
                {
                     Code=organisaction.OrganisationBaseCurrency.TaxCurrency.Code,
                     Symbole=organisaction.OrganisationBaseCurrency.TaxCurrency.Symbole,
                     Name=organisaction.OrganisationBaseCurrency.TaxCurrency.Name,
                     Id=organisaction.OrganisationBaseCurrency.TaxCurrency.Id
                     
                },
                
            };
            var fromDate =Utils.RessetFilterFromDate(new DateTime(int.Parse(month.Substring(0, 4)), int.Parse(month.Substring(4)), 1)).Value;
            var toDate =Utils.RessetFilterToDate(new DateTime(fromDate.Year, fromDate.Month,  DateTime.DaysInMonth(fromDate.Year, fromDate.Month))).Value;
            var invoices = (from u in context.Invoice
                           where u.IdNavigation.OrganisationId == organisationId &&
                           u.IdNavigation.Date >= fromDate && u.IdNavigation.Date <= toDate &&
                           u.IdNavigation.CustomerTransactionItem.Any(u=>u.TaxId!=null)
                           orderby u.IdNavigation.Date
                           select new Models.Invoice
                           {
                               BaseCurrencyExchangeRate = u.IdNavigation.BaseCurrencyExchangeRate,
                               Created = u.IdNavigation.Created,
                               CreatedBy = u.IdNavigation.CreatedBy,
                               Currency = new Currency
                               {
                                   Code = u.IdNavigation.Currency.Code,
                                   Id = u.IdNavigation.CurrencyId,
                                   Name = u.IdNavigation.Currency.Name,
                                   Symbole = u.IdNavigation.Currency.Symbole
                               },
                               CurrencyId = u.IdNavigation.CurrencyId,
                               Id = u.Id,
                               Customer = new Customer
                               {
                                   Id = u.IdNavigation.CustomerId,
                                   DisplayName = u.IdNavigation.Customer.DisplayName,
                                   LocalLegalName=u.IdNavigation.Customer.LocalLegalName,
                                   LegalName=u.IdNavigation.Customer.LegalName,
                                   TaxNumber=u.IdNavigation.Customer.TaxNumber
                               },
                               CustomerId = u.IdNavigation.CustomerId,
                               Date = u.IdNavigation.Date,
                               DueDate = u.IdNavigation.DueDate,
                               IsTaxIncome = u.IdNavigation.IsTaxIncome,
                               Note = u.IdNavigation.Note,
                               Number = u.IdNavigation.Number,
                               OrganisationId = u.IdNavigation.OrganisationId,
                               RefNo = u.IdNavigation.RefNo,
                               StatusId = u.StatusId,
                               Total = u.IdNavigation.Total,
                               TotalIncludeTax = u.IdNavigation.TotalIncludeTax,
                               TaxCurrencyExchangeRate = u.IdNavigation.TaxCurrencyExchangeRate,
                               Status = new InvoiceStatus
                               {
                                   Id = u.StatusId,
                                   Name = u.Status.Name
                               },
                               PaidBy = u.IdNavigation.PaidBy,
                               PaidDate = u.IdNavigation.PaidDate

                           }).ToList();
            result.Invoices = invoices;
            result.TotalInvoice = invoices.Sum(u => u.TotalIncludeTax *u.BaseCurrencyExchangeRate);
            var expenes = (from u in context.Expense
                           where u.OrganisationId == organisationId &&
                                 (u.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || u.ExpenseStatusId == (int)ExpenseStatusEnum.Billed) &&
                                 u.CloseDate >= fromDate && u.CloseDate <= toDate &&
                                 u.ExpenseItem.Any(u => u.TaxId != null)
                           orderby u.Date
                           select new Expense
                           {
                               ApprovedBy = u.ApprovedBy,
                               ApprovedDate = u.ApprovedDate,
                               BaseCurrencyExchangeRate = u.BaseCurrencyExchangeRate.Value,
                               BilledBy = u.BilledBy,
                               BilledDate = u.BilledDate,
                               Created = u.Created,
                               CreatedBy = u.CreatedBy,
                               Currency = new Currency
                               {
                                   Id = u.CurrencyId,
                                   Code = u.Currency.Code,
                                   Name = u.Currency.Name,
                                   Symbole = u.Currency.Symbole
                               },
                               Date = u.CloseDate.Value,
                               DeliveryDate = u.DeliveryDate,
                               Note = u.Note,
                               Id = u.Id,
                               CurrencyId = u.CurrencyId,
                               OrderNumber = u.Number,
                               OrganisationId = u.OrganisationId,
                               ExpenseStatus = new ExpenseStatus
                               {
                                   Id = u.ExpenseStatusId,
                                   Name = u.ExpenseStatus.Name,
                               },
                               ExpenseStatusId = u.ExpenseStatusId,
                               RefNo = u.RefNo,
                               TaxCurrencyExchangeRate = u.TaxCurrencyExchangeRate.Value,
                               Total = u.Total,
                               TotalIncludeTax = u.TotalIncludeTax,
                               VendorId = u.VendorId,
                               Vendor = new Vendor
                               {
                                   Id = u.VendorId,
                                   DisplayName = u.Vendor.DisplayName,
                                   LocalLegalName=u.Vendor.LocalLegalName,
                                   LegalName=u.Vendor.LegalName,
                                   TaxNumber=u.Vendor.TaxNumber
                               }

                           }).ToList();
            result.Expenses = expenes;
            result.TotalExpense = expenes.Sum(u => u.TotalIncludeTax*u.BaseCurrencyExchangeRate);
            var payroll = context.PayrollMonth.FirstOrDefault(u => u.Month == month && u.OrganisationId == organisationId);
            if (payroll == null)
            {
                result.Payroll = new PayrollMonthTax
                {
                    Payrolls = new List<PayrollTax>()
                };
                result.TotalEmployeeSalary = 0;
                result.IsPayrollRun = false;
            }
            else
            {
                result.IsPayrollRun = true;
                result.Payroll = new PayrollMonthTax
                {
                    AdditionalBenefitsRate = payroll.PayrollMonthTaxSalary.AdditionalBenefits,
                    NoneResidentRate = payroll.PayrollMonthTaxSalary.NoneResidentRate,
                    ChildOrSpouseAmount = payroll.PayrollMonthTaxSalary.ChildOrSpouseAmount,
                    TaxSalaryRanges = payroll.PayrollMonthTaxSalary.PayrollMonthTaxSalaryRange.OrderBy(u => u.TaxRate).Select(u => new TaxSalaryRange
                    {
                        Rate = u.TaxRate,
                        FromAmount = u.FromAmount,
                        ToAmount = u.ToAmount
                    }).ToList(),
                    Month = month,
                    OrganisationId = organisationId,
                    Id = payroll.Id,
                    EndDate = payroll.EndDate,
                    StartDate = payroll.StartDate,
                    Total = payroll.Total,
                    Currency =result.Currency,
                    TaxCurrency =result.TaxCurrency,
                    ExchangeRate = (decimal)payroll.ExchangeRate,
                    Payrolls = payroll.PayrollEmployee.Select(u => new PayrollTax
                    {
                        Date = u.Date,
                        DeductSalary = u.PayrollEmployeeTax.SalaryTax,
                        Employee = new Employee
                        {
                            Id = u.Id,
                            DisplayName = u.Employee.DisplayName,
                            JobTitle = u.Employee.JobTitle,
                            Country = new Country
                            {
                                Id = u.Employee.CountryId,
                                Alpha2Code = u.Employee.Country.Alpha2Code,
                                Alpha3Code = u.Employee.Country.Alpha3Code,
                                Name = u.Employee.Country.NameKh,
                                Nationality = u.Employee.Country.NationalityKh
                            },
                            IsConfederationThatHosts = u.PayrollEmployeeTax.ConfederationThatHosts,
                            NumberOfChild = u.PayrollEmployeeTax.NumberOfChilds,
                            IsResidentEmployee = u.PayrollEmployeeTax.IsResidentEmployee,
                            IDOrPassportNumber=u.Employee.IdorPassportNumber
                        },
                        EmployeeId = u.EmployeeId,
                        NumberOfChilds = u.PayrollEmployeeTax.NumberOfChilds,
                        OtherBenefit = u.PayrollEmployeeTax.OtherBenefit,
                        OtherBenefitTaxDeduct = u.PayrollEmployeeTax.OtherBenefitTaxDeduct,
                        Salary = u.PayrollEmployeeTax.Salary,
                        TransactionDate = u.TransactionDate,
                        Total = u.Total,
                        Id = u.Id

                    }).ToList()
                };
                result.TotalEmployeeSalary = result.Payroll.Total;
            }
            result.TotalInvoiceTaxInBaseCurrency = result.Invoices.Sum(u => (u.TotalIncludeTax - u.Total) * u.BaseCurrencyExchangeRate);
            result.TotalInvoiceTaxInTaxCurrency = result.Invoices.Sum(u => (u.TotalIncludeTax - u.Total) * u.TaxCurrencyExchangeRate);
            result.TotalExpenseTaxInBaseCurrency = result.Expenses.Sum(u => (u.TotalIncludeTax - u.Total) * u.BaseCurrencyExchangeRate);
            result.TotalExpenseTaxInTaxCurrency = result.Expenses.Sum(u => (u.TotalIncludeTax - u.Total) * u.TaxCurrencyExchangeRate);
            result.TotalEmployeeTaxInBaseCurrency = result.Payroll.Payrolls.Sum(u => u.DeductSalary + (u.OtherBenefitTaxDeduct == null ? 0 : u.OtherBenefitTaxDeduct.Value));
            result.TotalEmployeeTaxInTaxCurrency = result.Payroll.Payrolls.Sum(u => (u.DeductSalary + (u.OtherBenefitTaxDeduct == null ? 0 : u.OtherBenefitTaxDeduct.Value)) * result.Payroll.ExchangeRate);
            result.TotalPayToTax = result.TotalInvoiceTaxInBaseCurrency+ result.TotalExpenseTaxInBaseCurrency+ result.TotalEmployeeTaxInBaseCurrency;
            result.TotalPayToTaxInKHR = result.TotalInvoiceTaxInTaxCurrency+ result.TotalExpenseTaxInTaxCurrency+ result.TotalEmployeeTaxInTaxCurrency;
            return result;
        }

        private ProfitAndLostSummary GetProfitAndLostSummaryByMonth(ProfitAndLostSummaryFilter filter)
        {
            var organisationIds = filter.OrganisationIds;
            using var context = Context();
            var totalInvoice = (from invoice in context.Invoice
                                where organisationIds.Any(t => t == invoice.IdNavigation.OrganisationId) &&
                                invoice.IdNavigation.Date >= filter.FromDate && invoice.IdNavigation.Date <= filter.ToDate
                                select new
                                {
                                    invoice.IdNavigation.Date.Year,
                                    invoice.IdNavigation.Date.Month,
                                    invoice.IdNavigation.BaseCurrencyExchangeRate,
                                    invoice.IdNavigation.TotalIncludeTax
                                }).GroupBy(u => new { u.Month,u.Year})
                                .Select(u => new
                                {
                                    u.Key.Month,
                                    u.Key.Year,
                                    Total = u.Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate)
                                }).ToList();
            var totalOrders = (from order in context.PurchaseOrder
                               where organisationIds.Any(t => t == order.IdNavigation.OrganisationId) &&
                               order.IdNavigation.Date >= filter.FromDate && order.IdNavigation.Date <= filter.ToDate &&
                                 (order.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || order.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                               select new
                               {
                                   order.IdNavigation.Date.Year,
                                   order.IdNavigation.Date.Month,
                                   order.IdNavigation.BaseCurrencyExchangeRate,
                                   order.IdNavigation.TotalIncludeTax
                               }).GroupBy(u => new { u.Month, u.Year })
                                .Select(u => new
                                {
                                    u.Key.Month,
                                    u.Key.Year,
                                    Total = u.Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate)
                                }).ToList();
            var totalOrderExpenses = (from expense in context.Expense
                                      where organisationIds.Any(t => t == expense.OrganisationId) &&
                                      expense.Date >= filter.FromDate && expense.Date <= filter.ToDate &&
                                      expense.PurchaseOrder == null &&
                                        (expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || expense.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                                      select new
                                      {
                                          expense.Date.Year,
                                          expense.Date.Month,
                                          expense.BaseCurrencyExchangeRate,
                                          expense.TotalIncludeTax
                                      }).GroupBy(u => new { u.Month, u.Year })
                                .Select(u => new
                                {
                                    u.Key.Month,
                                    u.Key.Year,
                                    Total = u.Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate)
                                }).ToList();
            var totalEmployeeSalary= (from expense in context.PayrollEmployee
                                      where organisationIds.Any(t => t == expense.PayrollMonth.OrganisationId) &&
                                      expense.Date >= filter.FromDate && expense.Date <= filter.ToDate 
                                     
                                      select new
                                      {
                                          expense.Date.Year,
                                          expense.Date.Month,
                                          expense.Total
                                      }).GroupBy(u => new { u.Month, u.Year })
                                .Select(u => new
                                {
                                    u.Key.Month,
                                    u.Key.Year,
                                    Total = u.Sum(u => u.Total)
                                }).ToList();
            var months = new List<MonthGroup>();
            var date = filter.FromDate;
            int month = filter.FromDate.Value.Month;
            int year = filter.FromDate.Value.Year;
            while (month <= filter.ToDate.Value.Month && year <=filter.ToDate.Value.Year)
            {
                months.Add(new MonthGroup { Month=date.Value.Month,Year=date.Value.Year});
                date= date.Value.AddMonths(1);
                month = date.Value.Month;
                year = date.Value.Year;
            }

            var result = new ProfitAndLostSummary
            {
                Headers = months.Select(u => u.Month.ToString("00")+" - "+u.Year).ToList(),
                ProfitAndLostSummaryRowHeaders = new List<ProfitAndLostSummaryRowHeader>(),
                ProfitAndLostSummaryToal = new ProfitAndLostSummaryTotal
                {
                    Name = AppResource.GetResource("Profit & Lost"),
                    Values = new List<decimal>()
                }
            };
            result.Headers.Add(AppResource.GetResource("Total"));
            result.ProfitAndLostSummaryRowHeaders.Add(new ProfitAndLostSummaryRowHeader
            {
                Name = AppResource.GetResource("Income"),
                ProfitAndLostSummaryRows = new List<ProfitAndLostSummaryRow>
                {
                    new ProfitAndLostSummaryRow
                    {
                        Name=AppResource.GetResource("Invoice"),
                        Values=new List<decimal>()
                    }
                }
            });
            result.ProfitAndLostSummaryRowHeaders.Add(new ProfitAndLostSummaryRowHeader
            {
                Name = AppResource.GetResource("Expense"),
                ProfitAndLostSummaryRows = new List<ProfitAndLostSummaryRow>
                {
                    new ProfitAndLostSummaryRow
                    {
                        Name=AppResource.GetResource("Purchase"),
                        Values=new List<decimal>()
                    },
                    new ProfitAndLostSummaryRow
                    {
                        Name=AppResource.GetResource("Other Expense"),
                        Values=new List<decimal>()
                    },
                    new ProfitAndLostSummaryRow
                    {
                        Name=AppResource.GetResource("Employee Salary"),
                        Values=new List<decimal>()
                    }
                }
            }) ;
            int i = 0;
            foreach (var monthGroup in months)
            {
                var totalIncomeInOrganisation = totalInvoice.FirstOrDefault(u => u.Month == monthGroup.Month && u.Year==monthGroup.Year);
                var totalIncome = totalIncomeInOrganisation == null ? 0 : totalIncomeInOrganisation.Total;
                var totalOrderInOrganisation = totalOrders.FirstOrDefault(u => u.Month == monthGroup.Month && u.Year == monthGroup.Year);
                var totalOrder = totalOrderInOrganisation == null || totalOrderInOrganisation.Total == null ? 0 : totalOrderInOrganisation.Total.Value;
                var totalOtherExpenseInOrganisation = totalOrderExpenses.FirstOrDefault(u => u.Month == monthGroup.Month && u.Year == monthGroup.Year);
                var totalOtherExpense = totalOtherExpenseInOrganisation == null || totalOtherExpenseInOrganisation.Total == null ? 0 : totalOtherExpenseInOrganisation.Total.Value;


                var totalSalary = totalEmployeeSalary.FirstOrDefault(u => u.Month == monthGroup.Month && u.Year == monthGroup.Year);
                var totalSalaryValue = totalSalary == null ? 0 : totalSalary.Total;
                result.ProfitAndLostSummaryRowHeaders[0].ProfitAndLostSummaryRows[0].Values.Add(totalIncome);
                result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[0].Values.Add(totalOrder);
                result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[1].Values.Add(totalOtherExpense);
                result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[2].Values.Add(totalSalaryValue);
                var profit = totalIncome - totalOrder - totalOtherExpense- totalSalaryValue;
                if (result.ProfitAndLostSummaryToal.Values.Count < i + 1)
                {
                    result.ProfitAndLostSummaryToal.Values.Add(profit);
                }
                else
                {
                    result.ProfitAndLostSummaryToal.Values[i] += profit;
                }
                i++;
            }
            result.ProfitAndLostSummaryRowHeaders[0].Total = result.ProfitAndLostSummaryRowHeaders[0].ProfitAndLostSummaryRows.Sum(u => u.Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[1].Total = result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows.Sum(u => u.Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[0].ProfitAndLostSummaryRows[0].Values.Add(result.ProfitAndLostSummaryRowHeaders[0].ProfitAndLostSummaryRows[0].Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[0].Values.Add(result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[0].Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[1].Values.Add(result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[1].Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[2].Values.Add(result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[2].Values.Sum());
            result.ProfitAndLostSummaryToal.Values.Add(result.ProfitAndLostSummaryToal.Values.Sum());
            return result;

        }
        private ProfitAndLostSummary GetProfitAndLostSummaryByOrganisation(ProfitAndLostSummaryFilter filter)
        {
            var organisationIds = filter.OrganisationIds;
            using var context = Context();
            var totalInvoice = (from invoice in context.Invoice
                                where organisationIds.Any(t => t == invoice.IdNavigation.OrganisationId) &&
                                invoice.IdNavigation.Date >= filter.FromDate && invoice.IdNavigation.Date <= filter.ToDate
                                select new
                                {
                                    invoice.IdNavigation.OrganisationId,
                                    invoice.IdNavigation.BaseCurrencyExchangeRate,
                                    invoice.IdNavigation.TotalIncludeTax
                                }).GroupBy(u => u.OrganisationId)
                                .Select(u => new
                                {
                                    OrganisationId=u.Key,
                                    Total =u.Sum(u=>u.TotalIncludeTax*u.BaseCurrencyExchangeRate)
                                }).ToList();
            var totalOrders = (from order in context.PurchaseOrder
                                where organisationIds.Any(t => t == order.IdNavigation.OrganisationId) &&
                                order.IdNavigation.Date >= filter.FromDate && order.IdNavigation.Date <= filter.ToDate &&
                                  (order.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || order.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                               select new
                                {
                                    order.IdNavigation.OrganisationId,
                                    order.IdNavigation.BaseCurrencyExchangeRate,
                                    order.IdNavigation.TotalIncludeTax
                                }).GroupBy(u => u.OrganisationId)
                               .Select(u => new
                               {
                                   OrganisationId = u.Key,
                                   Total = u.Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate)
                               }).ToList();
            var totalOrderExpenses = (from expense in context.Expense
                               where organisationIds.Any(t => t == expense.OrganisationId) &&
                               expense.Date >= filter.FromDate && expense.Date <= filter.ToDate &&
                               expense.PurchaseOrder==null &&
                                 (expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || expense.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                               select new
                               {
                                   expense.OrganisationId,
                                   expense.BaseCurrencyExchangeRate,
                                   expense.TotalIncludeTax
                               }).GroupBy(u => u.OrganisationId)
                              .Select(u => new
                              {
                                  OrganisationId = u.Key,
                                  Total = u.Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate)
                              }).ToList();
            var totalEmployeeSalary = (from expense in context.PayrollEmployee
                                       where organisationIds.Any(t => t == expense.PayrollMonth.OrganisationId) &&
                                       expense.Date >= filter.FromDate && expense.Date <= filter.ToDate

                                       select new
                                       {
                                           expense.PayrollMonth.OrganisationId,
                                           expense.Total
                                       }).GroupBy(u => u.OrganisationId)
                             .Select(u => new
                             {
                                 OrganisationId=u.Key,
                                 Total = u.Sum(u => u.Total)
                             }).ToList();
            var organisations = context.Organisation.Where(u => organisationIds.Any(t => t == u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.DisplayName
                }).ToList();

            var result = new ProfitAndLostSummary
            {
                Headers = organisations.Select(u=>u.DisplayName).ToList(),
                ProfitAndLostSummaryRowHeaders=new List<ProfitAndLostSummaryRowHeader>(),
                ProfitAndLostSummaryToal=new ProfitAndLostSummaryTotal
                {
                    Name=AppResource.GetResource("Profit & Lost"),
                    Values=new List<decimal>()
                }
            };
            result.Headers.Add(AppResource.GetResource("Total"));
            result.ProfitAndLostSummaryRowHeaders.Add(new ProfitAndLostSummaryRowHeader
            {
                Name = AppResource.GetResource("Income"),
                ProfitAndLostSummaryRows = new List<ProfitAndLostSummaryRow>
                {
                    new ProfitAndLostSummaryRow
                    {
                        Name=AppResource.GetResource("Invoice"),
                        Values=new List<decimal>()
                    }
                }
            });
            result.ProfitAndLostSummaryRowHeaders.Add(new ProfitAndLostSummaryRowHeader
            {
                Name = AppResource.GetResource("Expense"),
                ProfitAndLostSummaryRows = new List<ProfitAndLostSummaryRow>
                {
                    new ProfitAndLostSummaryRow
                    {
                        Name=AppResource.GetResource("Purchase"),
                        Values=new List<decimal>()
                    },
                    new ProfitAndLostSummaryRow
                    {
                        Name=AppResource.GetResource("Other Expense"),
                        Values=new List<decimal>()
                    },
                    new ProfitAndLostSummaryRow
                    {
                         Name=AppResource.GetResource("Employee Salary"),
                        Values=new List<decimal>()
                    }
                }
            });
            int i = 0;
            foreach (var organisation in organisations)
            {
                var totalIncomeInOrganisation = totalInvoice.FirstOrDefault(u => u.OrganisationId == organisation.Id);
                var totalIncome = totalIncomeInOrganisation == null ? 0 : totalIncomeInOrganisation.Total;
                var totalOrderInOrganisation = totalOrders.FirstOrDefault(u => u.OrganisationId == organisation.Id);
                var totalOrder = totalOrderInOrganisation == null || totalOrderInOrganisation.Total==null ? 0 : totalOrderInOrganisation.Total.Value;
                var totalOtherExpenseInOrganisation = totalOrderExpenses.FirstOrDefault(u => u.OrganisationId == organisation.Id);
                var totalOtherExpense = totalOtherExpenseInOrganisation == null || totalOtherExpenseInOrganisation.Total == null ? 0 : totalOtherExpenseInOrganisation.Total.Value;
                var totalSalaray = totalEmployeeSalary.FirstOrDefault(u => u.OrganisationId == organisation.Id);
                var totalSalarayValue = totalSalaray == null ? 0 : totalSalaray.Total;
                result.ProfitAndLostSummaryRowHeaders[0].ProfitAndLostSummaryRows[0].Values.Add(totalIncome);
                result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[0].Values.Add(totalOrder);
                result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[1].Values.Add(totalOtherExpense);
                result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[2].Values.Add(totalSalarayValue);
                var profit = totalIncome - totalOrder - totalOtherExpense-totalSalarayValue;
                if (result.ProfitAndLostSummaryToal.Values.Count < i + 1)
                {
                    result.ProfitAndLostSummaryToal.Values.Add(profit);
                }
                else
                {
                    result.ProfitAndLostSummaryToal.Values[i] += profit;
                }
                i++;
            }
            result.ProfitAndLostSummaryRowHeaders[0].Total = result.ProfitAndLostSummaryRowHeaders[0].ProfitAndLostSummaryRows.Sum(u => u.Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[1].Total = result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows.Sum(u => u.Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[0].ProfitAndLostSummaryRows[0].Values.Add(result.ProfitAndLostSummaryRowHeaders[0].ProfitAndLostSummaryRows[0].Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[0].Values.Add(result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[0].Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[1].Values.Add(result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[1].Values.Sum());
            result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[2].Values.Add(result.ProfitAndLostSummaryRowHeaders[1].ProfitAndLostSummaryRows[2].Values.Sum());
            result.ProfitAndLostSummaryToal.Values.Add(result.ProfitAndLostSummaryToal.Values.Sum());
           
            return result;

        }
        private ProfitAndLostSummary GetProfitAndLostSummaryByTotalOnly(ProfitAndLostSummaryFilter filter)
        {
            using var context = Context();
            var organisationIds = filter.OrganisationIds;
            var totalInvoice = (from invoice in context.Invoice
                                where organisationIds.Any(t => t == invoice.IdNavigation.OrganisationId) &&
                                invoice.IdNavigation.Date >= filter.FromDate && invoice.IdNavigation.Date <= filter.ToDate
                                select invoice.IdNavigation.TotalIncludeTax * invoice.IdNavigation.BaseCurrencyExchangeRate
                              ).Sum();
            var totalPurchase= (from order in context.PurchaseOrder
                                where organisationIds.Any(t => t == order.IdNavigation.OrganisationId) &&
                                order.IdNavigation.Date >= filter.FromDate && order.IdNavigation.Date <= filter.ToDate &&
                                (order.IdNavigation.ExpenseStatusId==(int)ExpenseStatusEnum.Approved || order.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                                select order.IdNavigation.TotalIncludeTax * order.IdNavigation.BaseCurrencyExchangeRate
                              ).Sum();
            var totalOrderExpense = (from expense in context.Expense
                                 where organisationIds.Any(t => t == expense.OrganisationId) &&
                                 expense.PurchaseOrder==null &&
                                 expense.Date >= filter.FromDate && expense.Date <= filter.ToDate &&
                                 (expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || expense.ExpenseStatusId == (int)ExpenseStatusEnum.Billed)
                                 select expense.TotalIncludeTax * expense.BaseCurrencyExchangeRate
                             ).Sum();
            var totalEmployeeSalary = (from expense in context.PayrollEmployee
                                       where organisationIds.Any(t => t == expense.PayrollMonth.OrganisationId) &&
                                       expense.Date >= filter.FromDate && expense.Date <= filter.ToDate
                                       select expense.Total).Sum();
            return new ProfitAndLostSummary
            {
                Headers = new List<string>
                {
                    AppResource.GetResource("Total")
                },
                ProfitAndLostSummaryRowHeaders=new List<ProfitAndLostSummaryRowHeader>
                {
                    new ProfitAndLostSummaryRowHeader
                    {
                         Name=AppResource.GetResource("Income"),
                         ProfitAndLostSummaryRows=new List<ProfitAndLostSummaryRow>
                         {
                             new ProfitAndLostSummaryRow
                             {
                                 Name=AppResource.GetResource("Invoice"),
                                 Values=new List<decimal>
                                 {
                                     totalInvoice
                                 }
                             }
                         }
                    },
                    new ProfitAndLostSummaryRowHeader
                    {
                        Name=AppResource.GetResource("Expense"),
                        ProfitAndLostSummaryRows=new List<ProfitAndLostSummaryRow>
                        {
                            new ProfitAndLostSummaryRow
                            {
                                Name=AppResource.GetResource("Purchase"),
                                Values=new List<decimal>
                                {
                                    totalPurchase==null?0:totalPurchase.Value
                                }
                            },
                            new ProfitAndLostSummaryRow
                            {
                                Name=AppResource.GetResource("Other Expense"),
                                Values=new List<decimal>
                                {
                                    totalOrderExpense==null?0:totalOrderExpense.Value
                                }
                            },
                             new ProfitAndLostSummaryRow
                            {
                                Name=AppResource.GetResource("Employee Salary"),
                                Values=new List<decimal>
                                {
                                    totalEmployeeSalary
                                }
                            }
                        }
                    }
                },
                ProfitAndLostSummaryToal=new ProfitAndLostSummaryTotal
                {
                    Name= AppResource.GetResource("Profit & Lost"),
                    Values=new List<decimal>
                    {
                        totalInvoice - (totalOrderExpense==null?0:totalOrderExpense.Value)-(totalPurchase==null?0:totalPurchase.Value)-totalEmployeeSalary
                    }
                }
            };
        }
      
    }
}
