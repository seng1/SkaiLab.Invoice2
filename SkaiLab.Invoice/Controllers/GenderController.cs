using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class GenderController : ControllerBase
    {
        private readonly IGenderService genderService;

        public GenderController(IGenderService genderService)
        {
            this.genderService = genderService;
        }
        [HttpGet("[action]")]
        public IActionResult Gets()
        {
            return Ok(genderService.GetGenders());
        }
    }
}
