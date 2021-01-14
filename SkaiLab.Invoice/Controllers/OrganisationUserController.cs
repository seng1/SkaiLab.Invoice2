
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;
using System;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OrganisationUserController : ParentController
    {
        private readonly IOrganisationUserService organisationUserService;
        public OrganisationUserController(IOrganisationUserService organisationUserService) : base(organisationUserService)
        {
            this.organisationUserService = organisationUserService;
        }
        [HttpGet("[action]")]
        public IActionResult GetOrganisationUsers()
        {
            EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
            return Ok(organisationUserService.GetOrganisationUsers(organisationUserService.OrganisationId,organisationUserService.UserId));
        }
        [HttpGet("[action]")]
        public IActionResult GetMenuFeatures()
        {
            EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
            return Ok(organisationUserService.GetMenuFeatures());
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> InviteUser([FromBody] OrganisationUser organisationUser)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                var url = string.Format("{0}://{1}", Request.Scheme, Request.Host.Value);
                await organisationUserService.InviteUserAsync(organisationUser, organisationUserService.OrganisationId, organisationUserService.UserId, url);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
            
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> ResentInviatation(string email)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                var url = string.Format("{0}://{1}", Request.Scheme, Request.Host.Value);
                await organisationUserService.ResentInviatationAsync(organisationUserService.OrganisationId, organisationUserService.UserId, email, url);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
           
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrganisationUser(string email)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                return Ok(organisationUserService.GetOrganisationUser(organisationUserService.OrganisationId,organisationUserService.UserId,email));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpGet("[action]")]
        public IActionResult RemoveUser(string email)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                organisationUserService.RemoveOrganisationUser(organisationUserService.OrganisationId, organisationUserService.UserId, email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateUserRole([FromBody] OrganisationUser organisationUser)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                var url = string.Format("{0}://{1}", Request.Scheme, Request.Host.Value);
                await organisationUserService.UpdateUserRoleAsync(organisationUserService.OrganisationId, organisationUserService.UserId, organisationUser);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpGet("[action]")]
        public IActionResult GetLoginUser()
        {
           return Ok(organisationUserService.GetUser(organisationUserService.UserId));
        }
        [HttpPost("[action]")]
        public IActionResult UpdateLoginProfile([FromBody] User user)
        {
            user.Id = organisationUserService.UserId;
            organisationUserService.UpdateUser(user);
            return Ok();
        }
        [HttpPost("[action]")]
        public IActionResult UpdateUserLanguage([FromBody] User user)
        {
            user.Id = organisationUserService.UserId;
            organisationUserService.UpdateUserLanguage(user);
            return Ok();
        }

    }
}
