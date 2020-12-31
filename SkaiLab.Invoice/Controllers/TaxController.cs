using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TaxController : ParentController
    {
        private readonly ITaxService taxService;
        public TaxController(ITaxService taxService):base(taxService)
        {
            this.taxService = taxService;
        }
        [HttpGet("[action]")]
        public IActionResult Gets()
        {
            return Ok(taxService.GetTaxes(taxService.OrganisationId));
        }
        [HttpGet("[action]/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                return Ok(taxService.GetTax(id,taxService.OrganisationId));
            }
            catch(Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
           
        }
        [HttpPost("[action]")]
        public IActionResult Create([FromBody] Tax tax)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                tax.OrganisationId = taxService.OrganisationId;
                taxService.Create(tax);
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Tax tax)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                tax.OrganisationId = taxService.OrganisationId;
                taxService.Update(tax);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpGet("[action]")]
        public IActionResult GetTaxesIncludeComponent()
        {
            return Ok(taxService.GetTaxesIncludeComponent(taxService.OrganisationId));
        }
    }
}
