using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;
using System;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ExpensController : ParentController
    {
        private readonly IExpenseService expenseService;
        public ExpensController(IExpenseService expenseService) : base(expenseService, new int[] { (int)MenuFeatureEnum.ReadPurchaseSale, (int)MenuFeatureEnum.ReadWritePurchaseSale })
        {
            this.expenseService = expenseService;
        }
        [HttpGet("[action]/{id}")]
        public IActionResult RemoveAttachment(long id,string fileUrl)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ReadWritePurchaseSale);
                expenseService.RemoveAttachment(id, expenseService.OrganisationId, fileUrl);
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
                var fileUrl = expenseService.UploadAttachemnt(id, expenseService.OrganisationId, file);
                return Ok(new File { Url = fileUrl });
            }
            catch(Exception ex)
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
                expenseService.ChangeOfficialDocument(id, expenseService.OrganisationId, fileUrl);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
    }
}
