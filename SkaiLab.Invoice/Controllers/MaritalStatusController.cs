using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MaritalStatusController : ControllerBase
    {
        private readonly IMaritalStatusService maritalStatusService;
        public MaritalStatusController(IMaritalStatusService maritalStatusService)
        {
            this.maritalStatusService = maritalStatusService;
        }
        [HttpGet("[action]")]
        public IActionResult Gets()
        {
            return Ok(maritalStatusService.GetMaritalStatuses());
        }
    }
}
