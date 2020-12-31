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
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IOrganisationService _organisationService;
        public AccountController(IOrganisationService organisationService)
        {
            _organisationService = organisationService;
        }
        [HttpGet("[action]")]
        public IActionResult GetOrganisations()
        {
            return Ok(_organisationService.GetOrganisations(_organisationService.UserId));
        }
        [HttpGet("[action]")]
        public IActionResult GetWorkingOrganisation()
        {
            return Ok(_organisationService.GetWorkingOrganisation(_organisationService.UserId));
        }

    }
}
