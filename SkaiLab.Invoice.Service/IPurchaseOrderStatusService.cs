using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IPurchaseOrderStatusService:IService
    {
        List<ExpenseStatus> GetPurchaseOrderStatuses();
    }
}
