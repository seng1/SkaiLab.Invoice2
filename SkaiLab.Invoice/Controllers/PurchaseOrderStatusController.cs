using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PurchaseOrderStatusController : ControllerBase
    {
        private readonly IPurchaseOrderStatusService purchaseOrderStatusService;
        public PurchaseOrderStatusController(IPurchaseOrderStatusService purchaseOrderStatusService)
        {
            this.purchaseOrderStatusService = purchaseOrderStatusService;
        }
        [HttpGet("[action]")]
        public IActionResult Gets()
        {
            return Ok(purchaseOrderStatusService.GetPurchaseOrderStatuses()); ;
        }
    }
}
