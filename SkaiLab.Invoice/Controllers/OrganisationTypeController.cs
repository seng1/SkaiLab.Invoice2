using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OrganisationTypeController : BaseController
    {
        private readonly IOrganisationTypeService organisationTypeService;
        public OrganisationTypeController(IOrganisationTypeService organisationTypeService):base()
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
