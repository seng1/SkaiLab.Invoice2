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
            this.service = service;
            if (!service.IsValidLicense(service.OrganisationId))
            {
                throw new System.Exception(ErrorLicenseText());
            }
            if (!service.CheckPermission(service.UserId, service.OrganisationId, menuFeatureId))
            {
                throw new System.Exception("Unthorize");
            }
           
        }
        public ParentController(IService service, int[] menuFeatureIds)
        {
            this.service = service;
            if (!service.IsValidLicense(service.OrganisationId))
            {
                throw new System.Exception(ErrorLicenseText());
            }
            if (!service.CheckPermission(service.UserId, service.OrganisationId, menuFeatureIds))
            {
                throw new System.Exception("Unthorize");
            }
            
        }
        public ParentController(IService service)
        {
            this.service = service;
           
        }
        private string ErrorLicenseText()
        {
            if (service.IsKhmer)
            {
                return "អាជ្ញាប័ណ្ណរបស់ម្ចាស់ក្រុមហ៊ុននេះផុតសុពលភាពឬមិនទាន់បានទិញទេ។ ប្រសិនបើអ្នកជាម្ចាស់សូមធ្វើការទិញឥឡូវនេះ";
            }
            return "The license of the owner of this company already expire or not purchase yet. if your are the owner, please make a purchase now";
        }
        protected void EnsureHasPermission(int menuFeatureId)
        {
            if (!service.IsValidLicense(service.OrganisationId))
            {
                throw new System.Exception(ErrorLicenseText());
            }
            if (!service.CheckPermission(service.UserId, service.OrganisationId, menuFeatureId))
            {
                throw new System.Exception("Unthorize");
            }
        }
    }
}
