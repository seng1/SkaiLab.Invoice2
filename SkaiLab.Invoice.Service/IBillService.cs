using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Service
{
    public interface IBillService:IExpenseService
    {
        string GetBillOrderNumber(string organisationId);
        void Update(Expense purchaseOrder);
        void GetTotalPages(PurchaseOrderFilter filter);
        List<Expense> GetPurchaseOrders(PurchaseOrderFilter filter);
        void Create(Expense expense);
        Expense GetPurchase(string organisationId, long id);
        ExpenseForUpdate GetExpenseForUpdate(string organisationId, long id);
        void MarkAsBill(Expense expense);
        void MarkDelete(Expense expense);
        void MarkAsWaitingForApproval(List<long> expenseIds, string organisationId);
        void MarkAsApprove(List<Expense> expenses, string organisationId, string approvedBy);
        void MarkAsBill(List<long> expenseIds, string organisationId, string billBy);
        void MarkAsDelete(List<long> expenseIds, string organisationId, string deletBy);
        List<ExpenseStatus> GetExpenseStatuses(PurchaseOrderFilter filter);
        List<StatusOverview> GetStatusOverviews(string organisationId);
    }
}
