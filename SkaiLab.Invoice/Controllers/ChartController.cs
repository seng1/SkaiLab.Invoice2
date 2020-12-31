using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models.Filter;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ChartController : ControllerBase
    {
        private readonly IPieChartService pieChartService;
        public ChartController(IPieChartService pieChartService)
        {
            this.pieChartService = pieChartService;
        }
        [HttpPost("[action]")]
        public IActionResult GetProfiteAndLost([FromBody] DashboardFilter filter)
        {
            return Ok(pieChartService.GetProfiteAndLost(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetIncome([FromBody] DashboardFilter filter)
        {
            return Ok(pieChartService.GetIncome(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetExpense([FromBody] DashboardFilter filter)
        {
            return Ok(pieChartService.GetExpense(filter));
        }
    }
}
