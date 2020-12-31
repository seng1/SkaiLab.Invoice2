using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class PieChartService : Service, IPieChartService
    {
        public PieChartService(IDataContext context) : base(context)
        {

        }

        public PieChart GetExpense(DashboardFilter filter)
        {
            using var context = Context();
            var organisationIds = filter.OrganisationIds;
            var totalExpense = context.Expense.Where(u => (u.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || u.ExpenseStatusId == (int)ExpenseStatusEnum.Billed) &&
                                                         organisationIds.Any(t => t == u.OrganisationId) &&
                                                         u.CloseDate >= filter.FromDate &&
                                                         u.CloseDate <= filter.ToDate
                                                   )
               .Select(u => new { u.TotalIncludeTax, u.BaseCurrencyExchangeRate })
               .Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate);
            var totalEmployeeSalary = context.PayrollEmployee.Where(u => u.Date >= filter.FromDate && u.Date <= filter.ToDate)
                .Sum(u => u.Total);
            totalExpense += totalEmployeeSalary;

            var totalPendingBill = (from bill in context.Bill
                                    where organisationIds.Any(t => t == bill.IdNavigation.OrganisationId) &&
                                    bill.IdNavigation.CloseDate >= filter.FromDate &&
                                    bill.IdNavigation.CloseDate <= filter.ToDate &&
                                    (bill.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Draft || bill.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.WaitingForApproval)
                                    select bill.IdNavigation.TotalIncludeTax * bill.IdNavigation.BaseCurrencyExchangeRate
                                 ).Sum();

            var totalPendingPurchaseOrder = (from purchaseOrder in context.PurchaseOrder
                                    where organisationIds.Any(t => t == purchaseOrder.IdNavigation.OrganisationId) &&
                                    purchaseOrder.IdNavigation.CloseDate >= filter.FromDate &&
                                    purchaseOrder.IdNavigation.CloseDate <= filter.ToDate &&
                                    (purchaseOrder.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.Draft || purchaseOrder.IdNavigation.ExpenseStatusId == (int)ExpenseStatusEnum.WaitingForApproval)
                                    select purchaseOrder.IdNavigation.TotalIncludeTax * purchaseOrder.IdNavigation.BaseCurrencyExchangeRate
                     ).Sum();
            var totalUnpaid = (from expnse in context.Expense
                               where organisationIds.Any(t => t == expnse.OrganisationId) &&
                                 expnse.CloseDate >= filter.FromDate &&
                                 expnse.CloseDate <= filter.ToDate &&
                                 expnse.ExpenseStatusId == (int)ExpenseStatusEnum.Approved
                               select expnse.TotalIncludeTax * expnse.BaseCurrencyExchangeRate
                             ).Sum();
            return new PieChart
            {
                Labels = new List<string>
                {
                    AppResource.GetResource("Total Expense"),
                    AppResource.GetResource("Pending Bill"),
                    AppResource.GetResource("Pending PO"),
                     AppResource.GetResource("Un-Paid")
                },
                Values = new List<decimal>
                {
                   Math.Round(totalExpense.Value,0),
                   Math.Round(totalPendingBill.Value,0),
                   Math.Round(totalPendingPurchaseOrder.Value),
                   Math.Round(totalUnpaid.Value),
                }
            };


        }

        public PieChart GetIncome(DashboardFilter filter)
        {
            using var context = Context();
            var organisationIds = filter.OrganisationIds;
            var totalInvoice = context.Invoice.Where(u => organisationIds.Any(t => t == u.IdNavigation.OrganisationId)
                                        && u.IdNavigation.Date >= filter.FromDate
                                        && u.IdNavigation.Date <= filter.ToDate
                                      )
           .Select(u => new { u.IdNavigation.TotalIncludeTax, u.IdNavigation.BaseCurrencyExchangeRate })
           .Sum(u => u.BaseCurrencyExchangeRate * u.TotalIncludeTax);
            var totalOpeningQuote = context.Quote.Where(u => organisationIds.Any(t => t == u.OrganisationId) &&
                          u.Date >= filter.FromDate &&
                          u.Date <= filter.ToDate &&
                          (u.StatusId != (int)QuoteEnum.Delete && u.StatusId != (int)QuoteEnum.Declined && u.StatusId != (int)QuoteEnum.Invoiced)
                        ).Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate);
            var totalNotePaid = context.Invoice.Where(u => organisationIds.Any(t => t == u.IdNavigation.OrganisationId)
                                      && u.IdNavigation.Date >= filter.FromDate
                                      && u.IdNavigation.Date <= filter.ToDate 
                                      && u.StatusId==(int)InvoiceStatusEnum.WaitingForPayment
                                    ).Select(u => new { u.IdNavigation.TotalIncludeTax, u.IdNavigation.BaseCurrencyExchangeRate })
                                     .Sum(u => u.BaseCurrencyExchangeRate * u.TotalIncludeTax);
            return new PieChart
            {
                Labels = new List<string>
                {
                    AppResource.GetResource("Total Invoice"),
                    AppResource.GetResource("Pending Quote"),
                    AppResource.GetResource("Un-Paid Invoice")
                },
                Values = new List<decimal>
                {
                   Math.Round(totalInvoice,0),
                   Math.Round(totalOpeningQuote.Value,0),
                   Math.Round(totalNotePaid)
                }
            };

        }

        public PieChart GetProfiteAndLost(DashboardFilter filter)
        {
            using var context = Context();
            var organisationIds = filter.OrganisationIds;
            var totalInvoice = context.Invoice.Where(u => organisationIds.Any(t => t == u.IdNavigation.OrganisationId)
                                             && u.IdNavigation.Date >= filter.FromDate
                                             && u.IdNavigation.Date <= filter.ToDate
                                           )
                .Select(u => new { u.IdNavigation.TotalIncludeTax, u.IdNavigation.BaseCurrencyExchangeRate })
                .Sum(u => u.BaseCurrencyExchangeRate * u.TotalIncludeTax);
            var totalExpense = context.Expense.Where(u => (u.ExpenseStatusId == (int)ExpenseStatusEnum.Approved || u.ExpenseStatusId == (int)ExpenseStatusEnum.Billed) &&
                                                          organisationIds.Any(t => t == u.OrganisationId) &&
                                                          u.CloseDate >= filter.FromDate &&
                                                          u.CloseDate <= filter.ToDate
                                                    )
                .Select(u => new { u.TotalIncludeTax, u.BaseCurrencyExchangeRate })
                .Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate);
            var totalEmployeeSalary = context.PayrollEmployee.Where(u => u.Date >= filter.FromDate && u.Date <= filter.ToDate)
                .Sum(u => u.Total);
            totalExpense += totalEmployeeSalary;
            return new PieChart
            {
                Labels = new List<string>
                {
                   AppResource.GetResource("Income"),
                   AppResource.GetResource( "Expense"),
                   AppResource.GetResource("Profit & Lost")
                },
                Values=new List<decimal>
                {
                   Math.Round(totalInvoice,0),
                   Math.Round(totalExpense.Value,0),
                   Math.Round(totalInvoice-totalExpense.Value)
                }
            };


        }
    }
}
