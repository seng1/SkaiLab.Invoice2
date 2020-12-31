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
    public class InvoiceController : ParentController
    {
        private readonly IInvoiceService invoiceService;
        public InvoiceController(IInvoiceService invoiceService) : base(invoiceService, new int[] { (int)MenuFeatureEnum.ReadPurchaseSale, (int)MenuFeatureEnum.ReadWritePurchaseSale })
        {
            this.invoiceService = invoiceService;
        }
        [HttpPost("[action]")]
        public IActionResult GenerateInvoiceNumber([FromBody] Models.Invoice invoice)
        {
            invoice.Number = invoiceService.CreateInvoiceNumber(invoiceService.OrganisationId, invoice.IsTaxIncome, invoice.Date.Year, invoice.Date.Month);
            return Ok(invoice);
        }
        [HttpGet("[action]/{id}")]
        public IActionResult GenerateInvoiceFromQuote(long id)
        {
            return Ok(invoiceService.GetInvoiceFromQuote(id,invoiceService.OrganisationId));
        }
        [HttpPost("[action]")]
        public IActionResult CreateInvoiceFromQuote([FromBody] Models.Invoice invoice)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                invoice.OrganisationId = invoiceService.OrganisationId;
                invoice.CreatedBy = invoiceService.UserId;
                invoiceService.CreateInvoiceFromQuote(invoice);
                return Ok(invoice);
            }
            catch(Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
          
        }
        [HttpPost("[action]")]
        public IActionResult GetInvoices([FromBody] InvoiceFilter filter)
        {
            try
            {
                filter.OrganisationId = invoiceService.OrganisationId;
                return Ok(invoiceService.GetInvoices(filter));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult GetInvoiceStatuses([FromBody] InvoiceFilter filter)
        {
            try
            {
                filter.OrganisationId = invoiceService.OrganisationId;
                return Ok(invoiceService.GetInvoiceStatuses(filter));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult GetTotalPages([FromBody] InvoiceFilter filter)
        {
            try
            {
                filter.OrganisationId = invoiceService.OrganisationId;
                invoiceService.GetTotalPages(filter);
                return Ok(filter);
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult Create([FromBody] Models.Invoice invoice)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                invoice.OrganisationId = invoiceService.OrganisationId;
                invoice.CreatedBy = invoiceService.UserId;
                invoiceService.Create(invoice);
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpGet("[action]/{id}")]
        public IActionResult GetInvoice(long id)
        {
            return Ok(invoiceService.GetInvoice(id, invoiceService.OrganisationId));
        }
        [HttpGet("[action]")]
        public IActionResult GetOverView()
        {
            try
            {
                return Ok(invoiceService.GetStatusOverviews(invoiceService.OrganisationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]/{id}")]
        public IActionResult Pay(long id)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                invoiceService.Pay(id, invoiceService.OrganisationId, invoiceService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]")]
        public IActionResult PayAll([FromBody] List<long> ids)
        {
            try
            {
                invoiceService.Pay(ids, invoiceService.OrganisationId, invoiceService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]/{id}")]
        public IActionResult UploadFile(long id,[FromBody] Attachment attachment)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                invoiceService.UploadFile(id,invoiceService.OrganisationId,attachment);
                return Ok(attachment);
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]/{id}")]
        public IActionResult ChangeOfficialDocument(long id, string fileUrl)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                invoiceService.ChangeOfficialDocument(id, invoiceService.OrganisationId, fileUrl);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }

    }
}
