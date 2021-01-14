using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service.Subscription;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService planService;
        public PlanController(IPlanService planService)
        {
            this.planService = planService;
        }
        [HttpGet("[action]")]
        public IActionResult GetPlans()
        {
            return Ok(planService.GetPlans(1));
        }
        [HttpGet("[action]/{planId}")]
        public IActionResult CreateUserPlan(int planId,bool isYearLyPay)
        {
            try
            {
                planService.CreateUserPlan(planService.UserId, planId, isYearLyPay);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
    }
}
