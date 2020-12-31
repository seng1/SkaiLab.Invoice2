using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using SkaiLab.Invoice.Service;
using System;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class VendorController : ParentController
    {
        private readonly IVendorService vendorService;
        public VendorController(IVendorService vendorService) : base(vendorService, new int[] { (int)MenuFeatureEnum.ReadPurchaseSale, (int)MenuFeatureEnum.ReadWritePurchaseSale })
        {
            this.vendorService = vendorService;
        }
        [HttpPost("[action]")]
        public IActionResult GetVendors([FromBody] VendorFilter filter)
        {
            filter.OrganisationId = vendorService.OrganisationId;
            return Ok(vendorService.GetVendors(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetTotalPage([FromBody] VendorFilter filter)
        {
            filter.OrganisationId = vendorService.OrganisationId;
            vendorService.GetTotalPage(filter);
            return Ok(filter);
        }
        [HttpPost("[action]")]
        public IActionResult Add([FromBody] Vendor vendor)
        {
            EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
            vendor.OrganisationId = vendorService.OrganisationId;
            vendorService.Create(vendor);
            return Ok();
        }
        [HttpGet("[action]/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                return Ok(vendorService.GetVendor(id, vendorService.OrganisationId));
            }
            catch(Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpGet("[action]")]
        public IActionResult GetAllVendors()
        {
            try
            {
                return Ok(vendorService.GetVendors(vendorService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Vendor vendor)
        {
            EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
            vendor.OrganisationId = vendorService.OrganisationId;
            vendorService.Update(vendor);
            return Ok();
        }
    }
}
