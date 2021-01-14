using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;
using System;

namespace SkaiLab.Invoice.Controllers
{
    public class MakePaymentController : Controller
    {
        private readonly IPaymentService paymentService;
        public MakePaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }
        public IActionResult Index(string id,string culture=null)
        {
            var data = paymentService.GetUserLicenseInformationDetail(id);
            if (data.IsRenew)
            {
                var url = Url.Content("~/");
                return LocalRedirect(url + "MakePayment/Checkout?id=" + id+ "&culture="+culture);
            }
            ViewBag.priceUrl = paymentService.Option.LandingWebsiteUrl + "/home/price";
            ViewBag.culture = culture;
            return View(data);
        }
        [HttpGet]
        public IActionResult ApplyPromotionCode(long id,string code)
        {
            try
            {
                return Ok(paymentService.ApplyPromotionCode(id, code));

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult RemovePromotionCode(long id)
        {
            try
            {
                paymentService.RemovePromotionCode(id);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult SaveSubscription([FromBody] UserLicenseInformationDetail userLicense)
        {
            try
            {
                paymentService.SaveSubscription(userLicense);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public IActionResult Checkout(string id, string culture = null)
        {
            return View(paymentService.GetPaymentCheckout(id));
        }

    }
}
