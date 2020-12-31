using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService menuService;
        public MenuController(IMenuService menuService)
        {
            this.menuService = menuService;
        }
        [HttpGet("[action]")]
        public IActionResult Gets()
        {
            return Ok(menuService.GetMenus(menuService.UserId, menuService.OrganisationId));
        }
        [HttpGet("[action]")]
        public IActionResult GetPermission()
        {
            return Ok(menuService.GetPermission(menuService.UserId, menuService.OrganisationId));
        }
    }
}
