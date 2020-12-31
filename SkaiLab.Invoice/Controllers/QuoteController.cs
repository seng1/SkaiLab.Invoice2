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
    public class QuoteController : ParentController
    {
        private readonly IQuoteService quoteService;
        public QuoteController(IQuoteService quoteService) : base(quoteService, new int[] { (int)MenuFeatureEnum.ReadPurchaseSale, (int)MenuFeatureEnum.ReadWritePurchaseSale })
        {
            this.quoteService = quoteService;
        }
        [HttpPost("[action]")]
        public IActionResult Gets([FromBody] QuoteFilter filter)
        {
            filter.OrganisationId = quoteService.OrganisationId;
            return Ok(quoteService.GetQuotes(filter));
        }
        [HttpPost("[action]")]
        public IActionResult GetTotalPages([FromBody] QuoteFilter filter)
        {
            filter.OrganisationId = quoteService.OrganisationId;
            quoteService.GetTotalPages(filter);
            return Ok(filter);
        }
        [HttpPost("[action]")]
        public IActionResult GetQuoteStatuses([FromBody] QuoteFilter filter)
        {
            filter.OrganisationId = quoteService.OrganisationId;
            return Ok(quoteService.GetQuoteStatuses(filter));
        }
        [HttpGet("[action]")]
        public IActionResult GetLookupForCreae()
        {
            return Ok(quoteService.GetLookupForCreae(quoteService.OrganisationId));
        }
        [HttpPost("[action]")]
        public IActionResult Add([FromBody] Quote quote)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                quote.OrganisationId = quoteService.OrganisationId;
                quote.CreatedBy = quoteService.UserId;
                if (quote.StatusId == (int)QuoteEnum.Accepted)
                {
                    quote.AcceptedBy = quoteService.UserId;
                }
                quoteService.Create(quote);
                return Ok(quote);
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
                return Ok(quoteService.GetForUpdate(quoteService.OrganisationId, id));
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Quote quote)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                quote.OrganisationId = quoteService.OrganisationId;
                if (quote.StatusId == (int)QuoteEnum.Accepted)
                {
                    quote.AcceptedBy = quoteService.UserId;
                }
                quoteService.Update(quote);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpGet("[action]/{id}")]
        public IActionResult RemoveAttachment(long id, string fileUrl)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                quoteService.RemoveAttachment(id, quoteService.OrganisationId, fileUrl);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
        [HttpPost("[action]/{id}")]
        public IActionResult AddAttachment(long id, [FromBody] Attachment file)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                var fileUrl = quoteService.UploadAttachemnt(id, quoteService.OrganisationId, file.FileUrl,file.FileName);
                return Ok(new File { Url = fileUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult AcceptAll([FromBody] List<long> quoteIds)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                quoteService.Accept(quoteIds, quoteService.OrganisationId, quoteService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult DeclineAll([FromBody] List<long> quoteIds)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                quoteService.Decline(quoteIds, quoteService.OrganisationId, quoteService.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }

        }
        [HttpPost("[action]")]
        public IActionResult DeleteAll([FromBody] List<long> quoteIds)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                quoteService.Delete(quoteIds, quoteService.OrganisationId, quoteService.UserId);
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
                return Ok(quoteService.GetStatusOverviews(quoteService.OrganisationId));
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
                EnsureHasPermission((int)MenuFeatureEnum.ApprovaPayPurchaseSale);
                quoteService.ChangeOfficialDocument(id, quoteService.OrganisationId, fileUrl);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
    }
}
