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
    public class BillController : ParentController
    {
        private readonly IBillService billService;
        private readonly ICurrencyService currencyService;
        private readonly ITaxService taxService;
        private readonly ILocationService locationService;
        public  BillController(IBillService billService,
                ICurrencyService currencyService,
                ILocationService locationService,
                ITaxService taxService) : base(billService, new int[] { (int)MenuFeatureEnum.ReadPurchaseSale, (int)MenuFeatureEnum.ReadWritePurchaseSale })
        {
            this.billService = billService;
            this.currencyService = currencyService;
            this.taxService = taxService;
            this.locationService = locationService;
        }
        [HttpPost("[action]")]
        public IActionResult Gets([FromBody] PurchaseOrderFilter filter)
        {
            filter.OrganisationId = billService.OrganisationId;
            return Ok(billService.GetPurchaseOrders(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetTotalPages([FromBody] PurchaseOrderFilter filter)
        {
            filter.OrganisationId = billService.OrganisationId;
            billService.GetTotalPages(filter);
            return Ok(filter);
        }
        [HttpPost("[action]")]
        public IActionResult GetExpenseStatuses([FromBody] PurchaseOrderFilter filter)
        {
            filter.OrganisationId = billService.OrganisationId;
            return Ok(billService.GetExpenseStatuses(filter));
        }
        [HttpGet("[action]")]
        public IActionResult GetBillLookupForCreateOrUpdate()
        {
            try
            {
                var organisationId = billService.OrganisationId;
                var currencies = currencyService.GetCurrenciesWithExchangeRate(organisationId);
                var taxes = taxService.GetTaxesIncludeComponent(organisationId);
                var orderNumber = billService.GetBillOrderNumber(organisationId);
                var baseCurrencyId = billService.GetOrganisationBaseCurrencyId(organisationId);
                var taxCurrency = billService.GetTaxCurrency(organisationId);
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
                return Ok(billService.GetExpenseForUpdate(billService.OrganisationId,id));
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
                expense.OrganisationId = billService.OrganisationId;
                expense.CreatedBy = billService.UserId;
                if (expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved)
                {
                    expense.ApprovedBy = billService.UserId;
                }
                billService.Create(expense);
                expense.OrderNumber= billService.GetBillOrderNumber(billService.OrganisationId); 
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
                expense.OrganisationId = billService.OrganisationId;
                if (expense.ExpenseStatusId == (int)ExpenseStatusEnum.Approved)
                {
                    expense.ApprovedBy = billService.UserId;
                }
                billService.Update(expense);
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
                expense.OrganisationId = billService.OrganisationId;
                billService.MarkAsBill(expense);
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
                expense.OrganisationId = billService.OrganisationId;
                billService.MarkDelete(expense);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        public IActionResult CreateBillNumber()
        {
            try
            {
                var orderNumber = billService.GetBillOrderNumber(billService.OrganisationId);
                return Ok(new Expense { OrderNumber = orderNumber });
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult MarkBillsAsWaitingForApproval([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                billService.MarkAsWaitingForApproval(purchaseOrders, billService.OrganisationId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarkBillsOrdersAsApprove([FromBody] List<Expense> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                billService.MarkAsApprove(purchaseOrders, billService.OrganisationId, billService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarkBillsAsBill([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                billService.MarkAsBill(purchaseOrders, billService.OrganisationId, billService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult MarkBillsAsDelete([FromBody] List<long> purchaseOrders)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                billService.MarkAsDelete(purchaseOrders, billService.OrganisationId, billService.UserId);
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
                return Ok(billService.GetStatusOverviews(billService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
    }
}
