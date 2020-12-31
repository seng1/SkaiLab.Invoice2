using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class VendorExpenseController : ParentController
    {
        private readonly IVendorExpenseService vendorExpenseService;
        private readonly ICurrencyService currencyService;
        private readonly ITaxService taxService;
        private readonly ILocationService locationService;
        public VendorExpenseController(IVendorExpenseService vendorExpenseService,
               ICurrencyService currencyService,
               ILocationService locationService,
               ITaxService taxService) : base(vendorExpenseService, new int[] { (int)MenuFeatureEnum.ReadPurchaseSale, (int)MenuFeatureEnum.ReadWritePurchaseSale })
        {
            this.vendorExpenseService = vendorExpenseService;
            this.currencyService = currencyService;
            this.taxService = taxService;
            this.locationService = locationService;
        }
        [HttpPost("[action]")]
        public IActionResult Gets([FromBody] PurchaseOrderFilter filter)
        {
            filter.OrganisationId = vendorExpenseService.OrganisationId;
            return Ok(vendorExpenseService.GetExpenses(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetTotalPages([FromBody] PurchaseOrderFilter filter)
        {
            filter.OrganisationId = vendorExpenseService.OrganisationId;
            vendorExpenseService.GetTotalPages(filter);
            return Ok(filter);
        }
        [HttpGet("[action]")]
        public IActionResult GetLookupForCreateOrUpdate()
        {
            try
            {
                var organisationId = vendorExpenseService.OrganisationId;
                var currencies = currencyService.GetCurrenciesWithExchangeRate(organisationId);
                var taxes = taxService.GetTaxesIncludeComponent(organisationId);
                var orderNumber = vendorExpenseService.GetExpenseNumber(organisationId);
                var baseCurrencyId = vendorExpenseService.GetOrganisationBaseCurrencyId(organisationId);
                var taxCurrency = vendorExpenseService.GetTaxCurrency(organisationId);
                var locations = locationService.GetLocations(organisationId);
                return Ok(new { currencies, taxes, orderNumber, baseCurrencyId, taxCurrency, locations });
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]/{id}")]
        public IActionResult GetExpenseForUpdate(long id)
        {
            try
            {
                return Ok(vendorExpenseService.GetExpenseForUpdate(vendorExpenseService.OrganisationId, id));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult Create([FromBody] Expense expense)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                expense.OrganisationId = vendorExpenseService.OrganisationId;
                expense.CreatedBy = vendorExpenseService.UserId;
                if (expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved)
                {
                    expense.ApprovedBy = vendorExpenseService.UserId;
                }
                vendorExpenseService.Create(expense);
                expense.OrderNumber = vendorExpenseService.GetExpenseNumber(vendorExpenseService.OrganisationId);
                return Ok(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Expense expense)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                expense.OrganisationId = vendorExpenseService.OrganisationId;
                if (expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved)
                {
                    expense.ApprovedBy = vendorExpenseService.UserId;
                }
                vendorExpenseService.Update(expense);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult MarkAsBill([FromBody] Expense expense)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                expense.OrganisationId = vendorExpenseService.OrganisationId;
                vendorExpenseService.MarkAsBill(expense);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult MarkDelete([FromBody] Expense expense)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                expense.OrganisationId = vendorExpenseService.OrganisationId;
                vendorExpenseService.MarkDelete(expense);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult MarksAsWaitingForApproval([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                vendorExpenseService.MarkAsWaitingForApproval(purchaseOrders, vendorExpenseService.OrganisationId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarksAsApprove([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                vendorExpenseService.MarkAsApprove(purchaseOrders, vendorExpenseService.OrganisationId, vendorExpenseService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarksAsBill([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                vendorExpenseService.MarkAsBill(purchaseOrders, vendorExpenseService.OrganisationId, vendorExpenseService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarksAsDelete([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                vendorExpenseService.MarkAsDelete(purchaseOrders, vendorExpenseService.OrganisationId, vendorExpenseService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult GetExpenseStatuses([FromBody] PurchaseOrderFilter filter)
        {
            filter.OrganisationId = vendorExpenseService.OrganisationId;
            return Ok(vendorExpenseService.GetExpenseStatuses(filter));
        }
    }
}
