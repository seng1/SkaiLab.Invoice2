using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ReportController : ParentController
    {
        private readonly IReportService reportService;
        public ReportController(IReportService reportService):base(reportService,(int)MenuFeatureEnum.Report)
        {
            this.reportService = reportService;
        }
        [HttpPost("[action]")]
        public IActionResult GetProfitAndLostSummary([FromBody] ProfitAndLostSummaryFilter filter)
        {
            return Ok(reportService.GetProfitAndLostSummary(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetProfitAndLostDetail([FromBody] ReportFilter filter)
        {
            return Ok(reportService.GetProfitAndLostDetail(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetCustomerBalanceDetail([FromBody] ReportFilter filter)
        {
            return Ok(reportService.GetCustomerBalanceDetail(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetCustomerBalanceSummary([FromBody] ReportFilter filter)
        {
            return Ok(reportService.GetCustomerBalanceSummary(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetCustomerInvoices([FromBody] ReportFilter filter)
        {
            return Ok(reportService.GetCustomerInvoices(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetProductSaleSummaries([FromBody] ReportFilter filter)
        {
            return Ok(reportService.GetProductSaleSummaries(filter));
        }
        [HttpPost("[action]/{productId}")]
        public IActionResult GetProductSaleDetail(long productId, [FromBody] ReportFilter filter)
        {
            return Ok(reportService.GetProductSaleDetail(productId,filter));
        }
        [HttpGet("[action]")]
        public IActionResult GetProductInventoriesBalance(string searchText)
        {
            return Ok(reportService.GetProductInventoriesBalance(reportService.OrganisationId, searchText));
        }
        [HttpGet("[action]/{productId}")]
        public IActionResult GetProductInventoryByLocation(long productId)
        {
            return Ok(reportService.GetProductInventoryByLocation(reportService.OrganisationId, productId));
        }
        [HttpPost("[action]/{productId}")]
        public IActionResult GetInventoryHistories(long productId, [FromBody] InventoryHistoryFilter filter)
        {
            return Ok(reportService.GetInventoryHistories(reportService.OrganisationId, productId, filter));
        }
        [HttpGet("[action]/{month}")]
        public IActionResult GetTaxMonthly(string month)
        {
            return Ok(reportService.GetTaxMonthly(month, reportService.OrganisationId));
        }

        
    }
}
