using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;
using System;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OrganisationController : ParentController
    {
        private readonly IOrganisationService organisationService;
        public OrganisationController(IOrganisationService organisationService):base(organisationService)
        {
            this.organisationService = organisationService;
        }
        [HttpGet("[action]")]
        public IActionResult Get()
        {
            return Ok(organisationService.Get(organisationService.OrganisationId));
        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Organsation organisation)
        {
            EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
            if(organisation.Id!= organisationService.OrganisationId)
            {
                return BadRequest(new Error { ErrorText = "This organisation is not belong to you." });
            }
            organisationService.Update(organisation);
            return Ok();
        }
        [HttpGet("[action]")]
        public IActionResult GetBaseCurrency()
        {
            return Ok(organisationService.GetBaseCurrency(organisationService.OrganisationId));
        }
        [HttpGet("[action]")]
        public IActionResult GetTaxCurrency()
        {
            return Ok(organisationService.GetTaxCurrency(organisationService.OrganisationId));
        }
        [HttpPost("[action]")]
        public IActionResult Add([FromBody] Organsation organisation)
        {
            try
            {
                organisationService.Create(organisation, organisationService.UserId);
                return Ok(organisation);
            }
            catch(Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]/{id}")]
        public IActionResult ChangeWorkingOrganisation(string id)
        {
            try
            {
                organisationService.ChangeWorkingOrganisation(id, organisationService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]")]
        public IActionResult GetOrganisationsWithSameBaseCurrency()
        {
            return Ok(organisationService.GetOrganisationsWithSameBaseCurrency(organisationService.OrganisationId,organisationService.UserId));
        }
    }
}
