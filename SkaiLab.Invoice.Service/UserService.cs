using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SkaiLab.Invoice.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class UserService : Service, IUserService
    {
        private readonly IEmailService emailSender;
        public UserService(IDataContext context,
            IEmailService emailSender
            ) : base(context)
        {
            this.emailSender = emailSender;
        }

        public async Task<bool> ConfirmRegisterAsync(string userId, string verificationCode, UserManager<ApplicationUser> userManager, string companyName, int CurrencyId, bool isTax)
        {
            using var context = Context();
            var codeToken = context.AspNetUserTokensCode.FirstOrDefault(u => u.UserId == userId);
            if (codeToken == null || codeToken.Code != verificationCode || codeToken.Expire < CurrentCambodiaTime)
            {
                return false;
            }
            var user = await userManager.FindByIdAsync(userId);
            var aspCode = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await userManager.ConfirmEmailAsync(user, aspCode);
            context.AspNetUserTokensCode.Remove(codeToken);
            var organisation = new Dal.Models.Organisation
            {
                BussinessRegistrationNumber = "",
                Id = Guid.NewGuid().ToString(),
                Contact = new Dal.Models.Contact
                {
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    ContactName = context.AspNetUserClaims.FirstOrDefault(u => u.UserId == user.Id && u.ClaimType == ClaimTypes.Name).ClaimValue,
                    Website = "",
                    Address = "",
                    Id=Guid.NewGuid().ToString()
                },
                LegalLocalName = companyName,
                LegalName = companyName,
                DisplayName = companyName,
                DeclareTax = isTax,
                Description = "",
                LogoUrl = "",
                OrganisationTypeId = 1,
                TaxNumber = "",
                OrganisationInvoiceSetting = new Dal.Models.OrganisationInvoiceSetting
                {
                    TermAndConditionForInvoice = "",
                    TermAndConditionForPurchaseOrder = "",
                    TermAndConditionForQuote = ""
                },
                LineBusiness = "",

            };
            organisation.OrganisationUser.Add(new Dal.Models.OrganisationUser
            {
                IsOwner = true,
                RoleName = "Owner",
                UserId = user.Id,
                
            });
            var baseCurency = new Dal.Models.OrganisationCurrency
            {
                CurrencyId=CurrencyId,
                OrganisationId=organisation.Id
            };
            context.OrganisationCurrency.Add(baseCurency);
            organisation.OrganisationBaseCurrency = new Dal.Models.OrganisationBaseCurrency();
            organisation.OrganisationBaseCurrency.BaseCurrencyId=CurrencyId;
            organisation.OrganisationBaseCurrency.TaxCurrencyId = CurrencyId;
            var features = context.MenuFeature.Select(u => u.Id).ToList();
            foreach(var feature in features)
            {
                context.OrganisationUserMenuFeature.Add(new Dal.Models.OrganisationUserMenuFeature
                {
                    MenuFeatureId=feature,
                    OrganisationId=organisation.Id,
                    UserId=user.Id
                });
            }
            if (isTax)
            {
                var baseCurrency = context.Currency.FirstOrDefault(u => u.Id == CurrencyId);
                if (baseCurrency.Code != "KH")
                {
                    var taxCurerncy = context.Currency.FirstOrDefault(u => u.Code == "KHR");
                    organisation.OrganisationBaseCurrency.TaxCurrencyId = taxCurerncy.Id;
                    var taxCurrency = new Dal.Models.OrganisationCurrency
                    {
                        CurrencyId = taxCurerncy.Id,
                        OrganisationId=organisation.Id
                    };
                    context.OrganisationCurrency.Add(taxCurrency);
                    context.ExchangeRate.Add(new Dal.Models.ExchangeRate
                    {
                        FromOrganisationCurrency=baseCurency,
                        IsAuto=false,
                        ToOrganisationCurrency=taxCurrency,
                        ExchangeRate1=4000
                    });
                    context.ExchangeRate.Add(new Dal.Models.ExchangeRate
                    {
                        FromOrganisationCurrency = taxCurrency,
                        IsAuto = false,
                        ToOrganisationCurrency = baseCurency,
                        ExchangeRate1 =(decimal) 0.00025
                    });
                }
            }
            var plain = context.UserPlan.FirstOrDefault(u => u.UserId == user.Id);
            if (plain.IsTrail)
            {
                plain.Expire = CurrentCambodiaTime.AddMonths(1);
            }
            context.Organisation.Add(organisation);
            context.SaveChanges();
            return true;
        }

        public async Task<string> CreateUser(ApplicationUser applicationUser, UserManager<ApplicationUser> userManager, int planId, int subscriptionTypeId, bool isTrail)
        {
            using var context = Context();
            await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            var plan = context.Plan.FirstOrDefault(u => u.Id == planId);
            context.UserPlan.Add(new Dal.Models.UserPlan
            {
                Expire = null,
                PlanId = planId,
                ProjectId = 1,
                SubcriptionId = subscriptionTypeId,
                UserId = applicationUser.Id,
                Price=subscriptionTypeId==1?plan.MonthlyPrice:plan.YearlyPrice,
                RenewPrice=subscriptionTypeId==1?plan.MonthlyRenewPrice:plan.YearlyRenewPrice,
                IsTrail=isTrail
            });
            var code = GenerateSixDigit();
            context.AspNetUserTokensCode.Add(new Dal.Models.AspNetUserTokensCode
            {
                UserId = applicationUser.Id,
                Expire = CurrentCambodiaTime.AddMinutes(5),
                Code = code
            });
            context.SaveChanges();
            return code;

        }

        public Models.UserLicenseInformation GetUserLicenseInformation(string id)
        {
            using var context = Context();
            var user = context.UserPlan.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                var u = context.AspNetUsers.FirstOrDefault(u => u.Id == id);
                return new Models.UserLicenseInformation
                {
                    ExpireDate=null,
                    IsExpire=false,
                    IsTrail=false,
                    IsUserHasLicense=false,
                    PlanId=0,
                    SubscriptionId=0,
                    UserId=id,
                    User = new Models.User
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Name = u.AspNetUserClaims.FirstOrDefault(u => u.ClaimType == ClaimTypes.Name).ClaimValue
                    },
                    IsUserCompleteLicense=false,
                    IsAlertRenewLicense=false
                };
            }
            return new Models.UserLicenseInformation
            {
                UserId=user.UserId,
                SubscriptionId=user.SubcriptionId,
                PlanId=user.PlanId,
                ExpireDate=user.Expire,
                IsExpire=user.Expire<=CurrentCambodiaTime,
                IsTrail=user.IsTrail,
                IsUserHasLicense=true,
                IsUserCompleteLicense=!user.IsTrail && user.Expire!=null,
                IsAlertRenewLicense= !user.IsTrail && user.Expire != null && user.Expire.Value.Subtract(CurrentCambodiaTime).TotalDays<=15,
                User =new Models.User
                {
                    Id=user.User.Id,
                    Email=user.User.Email,
                    Name=user.User.AspNetUserClaims.FirstOrDefault(u=>u.ClaimType==ClaimTypes.Name).ClaimValue
                    
                }
            };
        }

        public bool IsCodeValid(string id, string verificationCode)
        {
            using var context = Context();
            var codeToken = context.AspNetUserTokensCode.FirstOrDefault(u => u.UserId == id);
            if (codeToken == null || codeToken.Code != verificationCode || codeToken.Expire < CurrentCambodiaTime)
            {
                return false;
            }
            return true;
        }

        public bool IsTrailUser(string id)
        {
            using var context = Context();
            return context.UserPlan.Any(u => u.UserId == id && u.IsTrail);
        }

        public async Task<bool> ResetUserPassword(string id, string verificationCode,string password, UserManager<ApplicationUser> userManager)
        {
            using var context = Context();
            var codeToken = context.AspNetUserTokensCode.FirstOrDefault(u => u.UserId == id);
            if(codeToken==null || codeToken.Code!=verificationCode || codeToken.Expire < CurrentCambodiaTime)
            {
                return false;
            }
            var user =await userManager.FindByIdAsync(id);
           var code= await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var result = await userManager.ResetPasswordAsync(user, code, password);
            if (!result.Succeeded)
            {
                return false;
            }
            context.AspNetUserTokensCode.Remove(codeToken);
            context.SaveChanges();
            return true;
        }

        public async Task SendNewCodeAsync(string id)
        {
            using var context = Context();
            var codeToken = context.AspNetUserTokensCode.FirstOrDefault(u => u.UserId == id);
            if (codeToken == null)
            {
                codeToken = new Dal.Models.AspNetUserTokensCode
                {
                    UserId=id
                };
                context.AspNetUserTokensCode.Add(codeToken);
            }
            codeToken.Code = GenerateSixDigit();
            codeToken.Expire = CurrentCambodiaTime.AddMinutes(5);
            context.SaveChanges();
            var email = context.AspNetUsers.FirstOrDefault(u => u.Id == id);
            var bodyKh = $"អរគុណសម្រាប់ការផ្ទៀងផ្ទាត់គណនី {email.Email} របស់អ្នក!<br/>កូដរបស់អ្នកគឺ៖ {codeToken.Code}";
            var body = $"Thank you for account verification for {email.Email}!<br/>Your code is:{codeToken.Code}";
            await emailSender.SendEmailAsync( new List<string> { email.Email },email.AspNetUserClaims.FirstOrDefault(u=>u.ClaimType==ClaimTypes.Name).ClaimValue, "លេខកូដបញ្ជាក់(verification code confirmation)", body, bodyKh);
        }

        private string GenerateSixDigit()
        {
            Random r = new Random();
            int randNum = r.Next(1000000);
            string sixDigitNumber = randNum.ToString("D6");
            return sixDigitNumber;
        }
    }
}
