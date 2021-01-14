using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IPaymentService:IService
    {
        UserLicenseInformationDetail GetUserLicenseInformationDetail(string id);
        UserLicenseInformationDetail ApplyPromotionCode(long id, string code);
        void RemovePromotionCode(long id);
        void SaveSubscription(UserLicenseInformationDetail userLicense);
        PaymentCheckout GetPaymentCheckout(string userId);
    }
}
