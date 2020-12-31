using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class PurchaseOrderStatusService:Service,IPurchaseOrderStatusService
    {
        public PurchaseOrderStatusService(IDataContext context) : base(context)
        {

        }

        public List<ExpenseStatus> GetPurchaseOrderStatuses()
        {
            using var context = Context();
            var statues = context.ExpenseStatus.Where(u=>u.Id!=(int)ExpenseStatusEnum.Delete).Select(u => new ExpenseStatus
            {
               Id=u.Id,
               Name=u.Name
            }).ToList();
            return statues;
        }
    }
}
