using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{

    [Route("api/[controller]")]
   
    public class AccountController : Controller
    {
        private readonly IOrganisationService _organisationService;
        private readonly IUserService userService;
        public AccountController(IOrganisationService organisationService, IUserService userService)
        {
            _organisationService = organisationService;
            this.userService = userService;
        }
        [Authorize]
        [HttpGet("[action]")]
        public IActionResult GetOrganisations()
        {
            return Ok(_organisationService.GetOrganisations(_organisationService.UserId));
        }
        [Authorize]
        [HttpGet("[action]")]
        public IActionResult GetWorkingOrganisation()
        {
            return Ok(_organisationService.GetWorkingOrganisation(_organisationService.UserId));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> SendNewCode(string id)
        {
            await userService.SendNewCodeAsync(id);
            return Ok();
        }
        [HttpGet("[action]")]
        public IActionResult GetUserLicenseInformation()
        {
            return Ok(userService.GetUserLicenseInformation(userService.UserId));
        }
    }
}
