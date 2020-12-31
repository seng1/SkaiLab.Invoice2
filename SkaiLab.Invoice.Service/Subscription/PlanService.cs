using SkaiLab.Invoice.Models.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service.Subscription
{
    public class PlanService:Service,IPlanService
    {
        public PlanService(IDataContext context) : base(context) { }

        public List<Plan> GetPlans(int projectPlanId)
        {
            using var context = Context();
            var khmer = IsKhmer;
            return context.Plan.Where(u => u.ProjectPlanId == projectPlanId)
                .OrderBy(u => u.Id)
                .Select(u => new Plan
                {
                    Id=u.Id,
                    ProjectPlanId=u.ProjectPlanId,
                    Name=khmer?u.NameKh: u.Name,
                    Price=u.Price,
                    YearDiscountRate=u.YearDiscount,
                    
                }).ToList();
        }
    }
}
