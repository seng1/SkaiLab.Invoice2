using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Report;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Service
{
    public interface IReportService:IService
    {
        ProfitAndLostSummary GetProfitAndLostSummary(ProfitAndLostSummaryFilter filter);
        ProfitAndLostDetail GetProfitAndLostDetail(ReportFilter filter);
        CustomerBalanceDetail GetCustomerBalanceDetail(ReportFilter filter);
        List<CustomerBalanceSummary> GetCustomerBalanceSummary(ReportFilter filter);
        List<Models.Invoice> GetCustomerInvoices(ReportFilter filter);
        List<ProductSaleSummary> GetProductSaleSummaries(ReportFilter reportFilter);
        ProductSaleDetail GetProductSaleDetail(long productId, ReportFilter reportFilter);
        List<Product> GetProductInventoriesBalance(string organisationId,string searchText);
        List<Product> GetProductInventoryByLocation(string organisationId, long productId);
        List<InventoryHistoryDetail> GetInventoryHistories(string organisationId, long productId, InventoryHistoryFilter reportFilter);
        TaxMonthly GetTaxMonthly(string month, string organisationId);
        List<ExpenseItem> GetExpenseItemsTax(List<long> expenseIds);
        List<CustomerTransactionItem> GetInvoiceItems(List<long> invoiceIds);
        List<Attachment> GetExpenseOfficalDocuments(List<long> expenseIds);
        List<Attachment> GetInvoiceOfficalDocuments(List<long> invoiceIds);
    } 
}
