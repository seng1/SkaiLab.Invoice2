using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OrganisationController : BaseController
    {
        private readonly IOrganisationService organisationService;
        public OrganisationController(IOrganisationService organisationService, IHttpContextAccessor contextAccessor):base(contextAccessor)
        {
            this.organisationService = organisationService;
        }
        [HttpGet("[action]")]
        public IActionResult Get()
        {
            return Ok(organisationService.Get(OrganisationId));
        }
    }
}
