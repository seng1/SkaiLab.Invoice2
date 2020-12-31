using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service.Subscription;

namespace SkaiLab.Invoice.Landing.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanService planService;
        public PlanController(IPlanService planService)
        {
            this.planService = planService;
        }
        [HttpGet]
        public IActionResult GetInvoicePlans()
        {
            return Ok(planService.GetPlans(1));
        }
    }
}
