using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;
using System;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PayrollController : ParentController
    {
        private readonly IPayrollService payrollService;
        public PayrollController(IPayrollService payrollService):base(payrollService,(int)MenuFeatureEnum.Payroll)
        {
            this.payrollService = payrollService;
        }
        [HttpGet("[action]")]
        public IActionResult GetMonthTax(string month)
        {
            return Ok(payrollService.GetMonthTax(payrollService.OrganisationId, month));
        }
        [HttpGet("[action]")]
        public IActionResult GetGetPayrollMonthNoneTax(string month)
        {
            return Ok(payrollService.GetPayrollMonth(payrollService.OrganisationId, month));
        }
        [HttpPost("[action]/{month}")]
        public IActionResult CreateOrUpdatePayrollTax(string month,[FromBody] PayrollMonthTax monthTax)
        {
            try
            {
                payrollService.CreatePayroll(monthTax, payrollService.OrganisationId, month);
                return Ok(monthTax);
            }
            catch(Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]/{month}")]
        public IActionResult CreateOrUpdatePayrollNoneTax(string month, [FromBody] PayrollMonthNoneTax monthTax)
        {
            try
            {
                payrollService.CreatePayroll(monthTax, payrollService.OrganisationId, month);
                return Ok(monthTax);
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
    }
}
