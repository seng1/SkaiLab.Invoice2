using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CustomerController : ParentController
    {
        private readonly ICustomerService customerService;
        public CustomerController(ICustomerService customerService) : base(customerService, new int[] { (int)MenuFeatureEnum.ReadPurchaseSale,(int)MenuFeatureEnum.ReadWritePurchaseSale})
        {
            this.customerService = customerService;
        }
        [HttpPost("[action]")]
        public IActionResult GetCustomers([FromBody] CustomerFilter filter)
        {
            filter.OrganisationId = customerService.OrganisationId;
            return Ok(customerService.GetCustomers(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetTotalPages([FromBody] CustomerFilter filter)
        {
            filter.OrganisationId = customerService.OrganisationId;
            customerService.GetTotalPages(filter);
            return Ok(filter);
        }
        [HttpPost("[action]")]
        public IActionResult Add([FromBody] Customer customer)
        {
            EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
            customer.OrganisationId = customerService.OrganisationId;
            customerService.Create(customer);
            return Ok();
        }
        [HttpGet("[action]/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                return Ok(customerService.GetCustomer(id, customerService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpGet("[action]")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(customerService.GetCustomers(customerService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Customer customer)
        {
            EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
            customer.OrganisationId = customerService.OrganisationId;
            customerService.Update(customer);
            return Ok();
        }
    }
}
