using SkaiLab.Invoice.Models.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service.Subscription
{
    public interface IPlanService:IService
    {
        List<Plan> GetPlans(int projectPlanId);
        void CreateUserPlan(string userId, int planId, bool isPayYearLy);
    }
}
