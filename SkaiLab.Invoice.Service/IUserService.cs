using Microsoft.AspNetCore.Identity;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Models;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IUserService:IService
    {
        Task<string> CreateUser(ApplicationUser applicationUser, UserManager<ApplicationUser> userManager, int planId, int subscriptionTypeId, bool isTrail);
        bool IsTrailUser(string id);
        Task<bool> ResetUserPassword(string id, string verificationCode,string password, UserManager<ApplicationUser> userManager);
        Task SendNewCodeAsync(string id);
        bool IsCodeValid(string id, string verificationCode);
        UserLicenseInformation GetUserLicenseInformation(string id);
        Task<bool> ConfirmRegisterAsync(string userId, string verificationCode, UserManager<ApplicationUser> userManager, string companyName, int CurrencyId, bool isTax);
    }
}
