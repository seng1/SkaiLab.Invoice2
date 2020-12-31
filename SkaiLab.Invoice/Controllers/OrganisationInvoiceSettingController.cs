using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OrganisationInvoiceSettingController : ParentController
    {
        private readonly IOrganisationInvoiceSettingService organisationInvoiceSettingService;
        public OrganisationInvoiceSettingController(IOrganisationInvoiceSettingService organisationInvoiceSetting):base(organisationInvoiceSetting)
        {
            this.organisationInvoiceSettingService = organisationInvoiceSetting;
        }
        [HttpGet("[action]")]
        public IActionResult Get()
        {
            return Ok(organisationInvoiceSettingService.GetOrganisationInvoiceSetting(organisationInvoiceSettingService.OrganisationId));
        }
        [HttpPost("[action]")]
        public IActionResult Save([FromBody] OrganisationInvoiceSetting organisationInvoiceSetting)
        {
            EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
            organisationInvoiceSetting.Id = organisationInvoiceSettingService.OrganisationId;
            organisationInvoiceSettingService.Save(organisationInvoiceSetting);
            return Ok();
        }
    }
}
