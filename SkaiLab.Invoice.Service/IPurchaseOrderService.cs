using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Service
{
    public interface IPurchaseOrderService:IExpenseService
    {
        string GetPurchaseOrderNumber(string organisationId);
        void Update(PurchaseOrder purchaseOrder);
        void GetTotalPages(PurchaseOrderFilter filter);
        List<PurchaseOrder> GetPurchaseOrders(PurchaseOrderFilter filter);
        void Create(PurchaseOrder purchaseOrder);
        PurchaseOrder GetPurchase(string organisationId, long id);
        PurchaseOrderForUpdate GetPurchaseOrderForUpdate(string organisationId, long id);
        void MarkAsBill(PurchaseOrder purchaseOrder);
        void MarkDelete(PurchaseOrder purchaseOrder);
        void MarkAsWaitingForApproval(List<long> purchaseOrderIds, string organisationId);
        void MarkAsApprove(List<PurchaseOrder> purchaseOrderapproves, string organisationId, string approvedBy);
        void MarkAsBill(List<long> purchaseOrderIds, string organisationId, string billBy);
        void MarkAsDelete(List<long> purchaseOrderIds, string organisationId, string deletBy);
        List<ExpenseStatus> GetExpenseStatuses(PurchaseOrderFilter filter);
        List<StatusOverview> GetStatusOverviews(string organisationId);


    }
}
