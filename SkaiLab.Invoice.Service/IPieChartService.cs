using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IPieChartService:IService
    {
        PieChart GetProfiteAndLost(DashboardFilter filter);
        PieChart GetIncome(DashboardFilter filter);
        PieChart GetExpense(DashboardFilter filter);
    }
}
