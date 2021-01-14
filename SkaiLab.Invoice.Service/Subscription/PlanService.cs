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

        public void CreateUserPlan(string userId, int planId, bool isPayYearLy)
        {
            using var context = Context();
            var userPlan = context.UserPlan.FirstOrDefault(u => u.UserId == userId);
            if (userPlan != null)
            {
                throw new Exception("User is already have plan");
            }
            var plan = context.Plan.FirstOrDefault(u => u.Id == planId);
            userPlan = new Dal.Models.UserPlan
            {
                UserId=UserId,
                PlanId=planId,
                Price=isPayYearLy?plan.YearlyPrice:plan.MonthlyPrice,
                IsTrail=false,
                Expire=null,
                RenewPrice=isPayYearLy?plan.YearlyRenewPrice:plan.MonthlyRenewPrice,
                ProjectId=1,
                SubcriptionId=isPayYearLy?2:1
            };
            context.UserPlan.Add(userPlan);
            context.SaveChanges();
        }

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
                    MontlyPrice=u.MonthlyPrice,
                    MontlyRenewPrice=u.MonthlyRenewPrice,
                    YearlyPrice=u.YearlyPrice,
                    YearlyRenewPrice=u.YearlyPrice,
                    YearlySavePercent=u.YearlySavePercent
                    
                }).ToList();
        }
    }
}
