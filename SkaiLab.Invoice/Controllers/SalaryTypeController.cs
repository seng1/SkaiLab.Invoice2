using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SalaryTypeController : ControllerBase
    {
        private readonly ISalaryTypeService salaryTypeService;

        public SalaryTypeController(ISalaryTypeService salaryTypeService)
        {
            this.salaryTypeService = salaryTypeService;
        }
        [HttpGet("[action]")]
        public IActionResult Gets()
        {
            return Ok(salaryTypeService.GetSalaryTypes(salaryTypeService.OrganisationId));
        }
    }
}
