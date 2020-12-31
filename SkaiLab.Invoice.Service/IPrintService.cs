using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Service
{
    public interface IPrintService:IService
    {
        Models.Quote GetQuote(long id);
        Models.PurchaseOrder GetPurchase(long id);
        Models.Expense GetBill(long id);
        Models.Expense GetExpense(long id);
        Models.Invoice GetInvoice(long id);
        Models.Receipt GetReceipt(long id,string purpose);
        bool IsTaxPayslip(long payrollId);
        PayrollTax GetPayrollTax(long id);
        PayrollNoneTax GetPayrollNoneTax(long id);

    }
}
