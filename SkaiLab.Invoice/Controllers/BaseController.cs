using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SkaiLab.Invoice.Controllers
{
    public class BaseController : Controller
    {
        protected string UserId = "";
        protected string DisplayName = "";
        protected string Email = "";
        protected string OrganisationId = "";
        public BaseController()
        {
          
        }
    }
}
