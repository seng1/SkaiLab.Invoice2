using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;
using System;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class LocationController : ParentController
    {
        private ILocationService locationService;
        public LocationController(ILocationService locationService):base(locationService)
        {
            this.locationService = locationService;
        }
        [HttpGet("[action]/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                return Ok(locationService.GetLocation(locationService.OrganisationId, id));
            }
            catch(Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]")]
        public IActionResult Gets()
        {
            try
            {
                return Ok(locationService.GetLocations(locationService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult Create([FromBody] Location location)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                locationService.Create(locationService.OrganisationId, location);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Location location)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                locationService.Update(locationService.OrganisationId, location);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
    }
}
