using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OrganisationTypeController : ControllerBase
    {
        private readonly IOrganisationTypeService organisationTypeService;
        public OrganisationTypeController(IOrganisationTypeService organisationTypeService)
        {
            this.organisationTypeService = organisationTypeService;
        }
        [HttpGet("[action]")]
        public IActionResult Gets()
        {
            return Ok(organisationTypeService.Gets());
        }
    }
}
