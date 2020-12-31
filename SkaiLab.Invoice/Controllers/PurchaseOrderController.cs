
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using SkaiLab.Invoice.Service;
using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PurchaseOrderController : ParentController
    {
        private readonly IPurchaseOrderService purchaseOrderService;
        private readonly ICurrencyService currencyService;
        private readonly ITaxService taxService;
        private readonly ILocationService locationService;
        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService,
                ICurrencyService currencyService,
                ILocationService locationService,
                ITaxService taxService) : base(purchaseOrderService, new int[] { (int)MenuFeatureEnum.ReadPurchaseSale, (int)MenuFeatureEnum.ReadWritePurchaseSale })
        {
            this.purchaseOrderService = purchaseOrderService;
            this.currencyService = currencyService;
            this.taxService = taxService;
            this.locationService = locationService;
        }
        [HttpPost("[action]")]
        public IActionResult Gets([FromBody] PurchaseOrderFilter filter)
        {
            filter.OrganisationId = purchaseOrderService.OrganisationId;
            return Ok(purchaseOrderService.GetPurchaseOrders(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetTotalPages([FromBody] PurchaseOrderFilter filter)
        {
            filter.OrganisationId = purchaseOrderService.OrganisationId;
            purchaseOrderService.GetTotalPages(filter);
            return Ok(filter);
        }
        [HttpPost("[action]")]
        public IActionResult GetExpenseStatuses([FromBody] PurchaseOrderFilter filter)
        {
            filter.OrganisationId = purchaseOrderService.OrganisationId;
            return Ok(purchaseOrderService.GetExpenseStatuses(filter));
        }
        [HttpGet("[action]")]
        public IActionResult GetPurchaseLookupForCreateOrUpdate()
        {
            try
            {
                var organisationId = purchaseOrderService.OrganisationId;
                var currencies = currencyService.GetCurrenciesWithExchangeRate(organisationId);
                var taxes = taxService.GetTaxesIncludeComponent(organisationId);
                var orderNumber = purchaseOrderService.GetPurchaseOrderNumber(organisationId);
                var baseCurrencyId = purchaseOrderService.GetOrganisationBaseCurrencyId(organisationId);
                var taxCurrency = purchaseOrderService.GetTaxCurrency(organisationId);
                var locations = locationService.GetLocations(organisationId);
                return Ok(new { currencies, taxes, orderNumber, baseCurrencyId, taxCurrency,locations });
            } 
            catch(Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]")]
        public IActionResult CreatePurchaseOrderNumber()
        {
            try
            {
                var orderNumber = purchaseOrderService.GetPurchaseOrderNumber(purchaseOrderService.OrganisationId);
                return Ok(new PurchaseOrder{OrderNumber=orderNumber  });
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult Create([FromBody] PurchaseOrder purchaseOrder)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                purchaseOrder.OrganisationId = purchaseOrderService.OrganisationId;
                purchaseOrder.CreatedBy = purchaseOrderService.UserId;
                if (purchaseOrder.ExpenseStatusId == (int)ExpenseStatusEnum.Approved)
                {
                    purchaseOrder.ApprovedBy = purchaseOrderService.UserId;
                }
                purchaseOrderService.Create(purchaseOrder);
                return Ok(purchaseOrder);
            }
            catch(Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
           
        }
        [HttpGet("[action]/{id}")]
        public IActionResult GetForUpdate(long id)
        {
            try
            {
                return Ok(purchaseOrderService.GetPurchaseOrderForUpdate(purchaseOrderService.OrganisationId, id));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] PurchaseOrder purchaseOrder)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                purchaseOrder.OrganisationId = purchaseOrderService.OrganisationId;
                if (purchaseOrder.ExpenseStatusId == (int)ExpenseStatusEnum.Approved)
                {
                    purchaseOrder.ApprovedBy = purchaseOrderService.UserId;
                }
                purchaseOrderService.Update(purchaseOrder);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarkAsBill([FromBody] PurchaseOrder purchaseOrder)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                purchaseOrder.OrganisationId = purchaseOrderService.OrganisationId;
                purchaseOrderService.MarkAsBill(purchaseOrder);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarkDelete([FromBody] PurchaseOrder purchaseOrder)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                purchaseOrder.OrganisationId = purchaseOrderService.OrganisationId;
                purchaseOrder.DeletedBy = purchaseOrderService.UserId;
                purchaseOrderService.MarkDelete(purchaseOrder);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarkPurchaseOrdersAsWaitingForApproval([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                purchaseOrderService.MarkAsWaitingForApproval(purchaseOrders,purchaseOrderService.OrganisationId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarkPurchaseOrdersAsApprove([FromBody] List<PurchaseOrder> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                purchaseOrderService.MarkAsApprove(purchaseOrders, purchaseOrderService.OrganisationId,purchaseOrderService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarkPurchaseOrdersAsBill([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                purchaseOrderService.MarkAsBill(purchaseOrders, purchaseOrderService.OrganisationId, purchaseOrderService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarkPurchaseOrdersAsDelete([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                purchaseOrderService.MarkAsDelete(purchaseOrders, purchaseOrderService.OrganisationId, purchaseOrderService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpGet("[action]")]
        public IActionResult GetOverView()
        {
            try
            {
                return Ok(purchaseOrderService.GetStatusOverviews(purchaseOrderService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
    }
}
