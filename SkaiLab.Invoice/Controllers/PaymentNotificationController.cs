using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkaiLab.Invoice.Models.Payway;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentNotificationController : ControllerBase
    {
        private readonly IPaywayService paywayService;
        public PaymentNotificationController(IPaywayService paywayService)
        {
            this.paywayService = paywayService;
        }
        [HttpPost("[action]/kfwGYhTxnsyT7Q_1y9mV7r")]
        public async Task<ActionResult> PaywayPushBackNotification()
        {
            RequestComplete requestComplete = null;
            using (StreamReader stream = new StreamReader(Request.Body))
            {
                string body = await stream.ReadToEndAsync();
                body = body.Split('{')[1].Split('}')[0];
                requestComplete = JsonConvert.DeserializeObject<RequestComplete>("{" + body + "}");
            }
            if (requestComplete == null)
            {
                return BadRequest("Error on casting model");
            }
            try
            {
                await paywayService.ProcessPayAsync(requestComplete);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }
    }
}
