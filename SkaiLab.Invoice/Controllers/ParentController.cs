using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    public class ParentController : ControllerBase
    {
        private readonly IService service;
        public ParentController(IService service,int menuFeatureId)
        {
            if (!service.CheckPermission(service.UserId, service.OrganisationId, menuFeatureId))
            {
                throw new System.Exception("Unthorize");
            }
            this.service = service;
        }
        public ParentController(IService service, int[] menuFeatureIds)
        {
            if (!service.CheckPermission(service.UserId, service.OrganisationId, menuFeatureIds))
            {
                throw new System.Exception("Unthorize");
            }
            this.service = service;
        }
        public ParentController(IService service)
        {
            this.service = service;
        }
        protected void EnsureHasPermission(int menuFeatureId)
        {
            if (!service.CheckPermission(service.UserId, service.OrganisationId, menuFeatureId))
            {
                throw new System.Exception("Unthorize");
            }
        }
    }
}
