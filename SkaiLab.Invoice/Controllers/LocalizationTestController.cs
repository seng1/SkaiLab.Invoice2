using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationTestController : ControllerBase
    {
        private readonly IAppResource _sharedLocalizer;
        public LocalizationTestController(IAppResource sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public string Get()
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-US")),
                new CookieOptions()
            );
            string result = _sharedLocalizer.GetResource("Hi");
            return result;
        }
    }
}
