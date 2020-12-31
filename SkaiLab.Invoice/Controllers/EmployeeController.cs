using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class EmployeeController : ParentController
    {
        private readonly IEmployeeService employeeService;
        public EmployeeController(IEmployeeService employeeService):base(employeeService)
        {
            this.employeeService = employeeService;
        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Employee employee)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                employee.OrganisationId = employeeService.OrganisationId;
                employeeService.Update(employee);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpPost("[action]")]
        public IActionResult Add([FromBody] Employee employee)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                employee.OrganisationId = employeeService.OrganisationId;
                employeeService.Add(employee);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpGet("[action]/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                return Ok(employeeService.Get(id,employeeService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpGet("[action]")]
        public IActionResult Gets(string searchText)
        {
            try
            {
                return Ok(employeeService.GetEmployees(searchText, employeeService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
    }
}
