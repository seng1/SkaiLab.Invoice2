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
    public class ProductController : ParentController
    {
        private readonly IProductService productService;
        public ProductController(IProductService productService) : base(productService, new int[] { (int)MenuFeatureEnum.ReadPurchaseSale, (int)MenuFeatureEnum.ReadWritePurchaseSale })
        {
            this.productService = productService;
        }
        [HttpPost("[action]")]
        public IActionResult Gets([FromBody] ProductFilter filter)
        {
            filter.OrganisationId = productService.OrganisationId;
            return Ok(productService.GetProducts(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetTotalPages([FromBody] ProductFilter filter)
        {
            filter.OrganisationId = productService.OrganisationId;
            productService.GetTotalPages(filter);
            return Ok(filter);
        }
        [HttpPost("[action]")]
        public IActionResult Create([FromBody] Product product)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                product.OrganisationId = productService.OrganisationId;
                productService.Create(product,productService.UserId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Product product)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                product.OrganisationId = productService.OrganisationId;
                productService.Update(product);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                return Ok(productService.GetProduct(id,productService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]/{id}/{locationId}")]
        public IActionResult GetInventoryQty(long id,long locationId)
        {
            try
            {
                var inventoryQty = productService.GetInventoryQty(id, locationId, productService.OrganisationId);
                return Ok(inventoryQty);
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]")]
        public IActionResult GetProductsForPurchase()
        {
            try
            {
                return Ok(productService.GetProductsForPurchase(productService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]")]
        public IActionResult GetProductsForSale()
        {
            try
            {
                return Ok(productService.GetProductsForSale(productService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
    }
}
