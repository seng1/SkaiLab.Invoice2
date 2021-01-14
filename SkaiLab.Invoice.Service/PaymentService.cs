using SkaiLab.Invoice.Dal.Models;
using SkaiLab.Invoice.Models;
using System;
using System.Linq;
using System.Security.Claims;
namespace SkaiLab.Invoice.Service
{
    public class PaymentService:Service,IPaymentService
    {
        public PaymentService(IDataContext context) : base(context)
        {

        }

        public UserLicenseInformationDetail ApplyPromotionCode(long id, string code)
        {
            using var context = Context();
            var coupon = context.Coupon.FirstOrDefault(u => u.Code == code);
            if (coupon == null)
            {
                throw new Exception("Promotion code not found");
            }
            if (coupon.ExpireDate < CurrentCambodiaTime)
            {
                throw new Exception("Promotion code expired");
            }
            if (coupon.NumberOfUsed >= coupon.NumberCanUse)
            {
                throw new Exception("Promotion code is not valid");
            }
            var payment = context.UserPayment.FirstOrDefault(u => u.Id == id);
            payment.DiscountRate = coupon.Rate;
            payment.TotalAfterDiscount = payment.Price - ((payment.DiscountRate.Value * payment.Price) / 100);
            payment.TaxAmount = payment.IsTax?(payment.TotalAfterDiscount * 10) / 100:0;
            payment.TotalIncludeTax = payment.TotalAfterDiscount + payment.TaxAmount;
            coupon.NumberOfUsed += 1;
            payment.CouponCode = code;
            context.SaveChanges();
            return new UserLicenseInformationDetail
            {
                CouponCode = coupon.Code,
                DiscountRate = coupon.Rate
            };
        }

        public PaymentCheckout GetPaymentCheckout(string userId)
        {
            using var context = Context();
            var userPlan = context.UserPlan.FirstOrDefault(u => u.UserId == userId);
            var payment = context.UserPayment.FirstOrDefault(u => u.UserId == userId && !u.IsPaid);
            if (userPlan.Expire == null)
            {
                if (payment == null)
                {
                    throw new Exception("Payment not generate yet.");
                }
            }
            if (payment == null)
            {
                var taxAmount = Option.IsTax ? (userPlan.RenewPrice * 10) / 100 : 0;
                payment = new Dal.Models.UserPayment
                {
                    CouponCode=null,
                    Created=CurrentCambodiaTime,
                    DiscountRate=null,
                    PlanId=userPlan.PlanId,
                    SubscriptionTypeId=userPlan.SubcriptionId,
                    IsPaid=false,
                    Price=userPlan.RenewPrice,
                    TaxAmount=taxAmount,
                    TotalAfterDiscount=userPlan.Price,
                    TotalIncludeTax=userPlan.Price+taxAmount,
                    UserId=userPlan.UserId
                };
                context.UserPayment.Add(payment);
            }
            if (userPlan.Expire == null || userPlan.IsTrail)
            {
                payment.Description = $"ការទិញ Skai Invoice សំរាប់គំរោង {userPlan.Plan.Name} សំរាប់រយះពេល​ ១ ";
                payment.Description += userPlan.SubcriptionId == 1 ? "ខែ " : "ឆ្នាំ ";
                payment.Description += $"ចាប់​ពី {CurrentCambodiaTime:dd/MM/yyyy} រហូតដល់ " +
                    (userPlan.SubcriptionId == 1 ? CurrentCambodiaTime.AddMonths(1).ToString("dd/MM/yyyy") :
                    CurrentCambodiaTime.AddYears(1).ToString("dd/MM/yyyy"));
                payment.Description += Environment.NewLine;
                payment.Description += $"Purchase Skai Invoice for plan {userPlan.Plan.Name} for ១ ";
                payment.Description += userPlan.SubcriptionId == 1 ? "month " : "year ";
                payment.Description += $"from {CurrentCambodiaTime:dd/MM/yyyy} to " +
                    (userPlan.SubcriptionId == 1 ? CurrentCambodiaTime.AddMonths(1).ToString("dd/MM/yyyy") :
                    CurrentCambodiaTime.AddYears(1).ToString("dd/MM/yyyy"));

            }
            else
            {
                payment.Description = $"ការបន្តជាថ្មី Skai Invoice សំរាប់គំរោង {userPlan.Plan.Name} សំរាប់រយះពេល​ ១ ";
                payment.Description += userPlan.SubcriptionId == 1 ? "ខែ " : "ឆ្នាំ ";
                payment.Description += $"ចាប់​ពី {userPlan.Expire.Value:dd/MM/yyyy} រហូតដល់ " +
                    (userPlan.SubcriptionId == 1 ?userPlan.Expire.Value.AddMonths(1).ToString("dd/MM/yyyy") :
                    userPlan.Expire.Value.AddYears(1).ToString("dd/MM/yyyy"));
                payment.Description += Environment.NewLine;
                payment.Description += $"Renewal Skai Invoice for plan {userPlan.Plan.Name} for ១ ";
                payment.Description += userPlan.SubcriptionId == 1 ? "ខែ " : "ឆ្នាំ" ;
                payment.Description += $"from {userPlan.Expire.Value:dd/MM/yyyy} to " +
                    (userPlan.SubcriptionId == 1 ? userPlan.Expire.Value.AddMonths(1).ToString("dd/MM/yyyy") :
                    userPlan.Expire.Value.AddYears(1).ToString("dd/MM/yyyy"));
            }
            if (payment.UserPaymentPayway != null)
            {
                context.PaywayTransactionLog.Add(new Dal.Models.PaywayTransactionLog
                {
                    Created=CurrentCambodiaTime,
                    TransactionId=payment.UserPaymentPayway.TransactionId,
                    UserPaymentId=payment.Id
                });
            }
            payment.UserPaymentPayway = new Dal.Models.UserPaymentPayway
            {
                TransactionId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
            };
            context.SaveChanges();
            var customerName = userPlan.User.AspNetUserClaims.FirstOrDefault(u => u.ClaimType == ClaimTypes.Name).ClaimValue;
            var result= new PaymentCheckout
            {
                UserLicenseInformationDetail=new UserLicenseInformationDetail
                {
                    UserId=userPlan.UserId,
                    TotalIncludeTax=payment.TotalIncludeTax,
                    TotalAfterDiscount=payment.TotalAfterDiscount,
                    CouponCode=payment.CouponCode,
                    DiscountRate=payment.DiscountRate,
                    SubscriptionId=payment.SubscriptionTypeId,
                    PaymentId=payment.Id,
                    ExpireDate=userPlan.Expire,
                    TotalDiscount=payment.DiscountRate==null?null:(double?)payment.DiscountRate.Value * payment.Price/100,
                    PlanId=payment.PlanId,
                    TaxAmount=payment.TaxAmount,
                    Total=payment.Price,
                    PaymentDescription=payment.Description,
                    IsTax=payment.IsTax
                },
                PayWayCreateTransaction=new Models.Payway.CreateTransaction
                {
                    Email=userPlan.User.Email,
                    Phone=userPlan.User.AspNetUserClaims.FirstOrDefault(u=>u.ClaimType==ClaimTypes.MobilePhone).ClaimValue,
                    Firstname=customerName.Split(' ')[0],
                    Lastname=customerName.Contains(' ')?customerName.Split(' ')[1]:customerName,
                    TransactionId=payment.UserPaymentPayway.TransactionId,
                    Amount=payment.TotalIncludeTax.ToString("0.00").Replace(",", "."),
                    Items= Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"item[0][{payment.Description}],item[0][1],item[0][{payment.TotalIncludeTax}]")),
                    ContinueSuccessUrl= "https://khdomain-uat.azurewebsites.net/buydomain/PurchasThank?id="+payment.UserPaymentPayway.TransactionId,
                    ApiUrl=Option.PayWay.ApiUrl,
                }
            };
            result.PayWayCreateTransaction.Hash = Utils.SHA512_ComputeHash($"{Option.PayWay.MerchantId}{result.PayWayCreateTransaction.TransactionId}{result.PayWayCreateTransaction.Amount}", Option.PayWay.ApiKey);
            return result;
        }

        public UserLicenseInformationDetail GetUserLicenseInformationDetail(string id)
        {
            using var context = Context();
            var userPlan = context.UserPlan.FirstOrDefault(u => u.UserId == id);
            var payment = context.UserPayment.FirstOrDefault(u => u.UserId == id && !u.IsPaid);
            if (payment == null)
            {
                payment = new Dal.Models.UserPayment
                {
                    IsPaid=false,
                    CouponCode=null,
                    Created=CurrentCambodiaTime,
                    PlanId=userPlan.PlanId,
                    SubscriptionTypeId=userPlan.SubcriptionId,
                    UserId=userPlan.UserId,
                    Price=userPlan.Price,
                    DiscountRate=null,
                    TaxAmount=Option.IsTax? userPlan.Price * 10/100:0,
                    TotalAfterDiscount=userPlan.Price,
                    TotalIncludeTax= (Option.IsTax ? userPlan.Price * 10 / 100 : 0) +userPlan.Price,
                    IsTax=Option.IsTax,
                   
                };
                context.UserPayment.Add(payment);
                context.SaveChanges();
            }
            var khmer = IsKhmer;
            return new UserLicenseInformationDetail
            {
                UserId = userPlan.UserId,
                CouponCode = payment.CouponCode,
                DiscountRate = payment.DiscountRate,
                TotalIncludeTax = payment.TotalIncludeTax,
                TotalAfterDiscount = payment.TotalAfterDiscount,
                TotalDiscount=payment.DiscountRate==null? (double?)null : (double?)(payment.DiscountRate.Value * payment.Price)/100,
                TaxAmount = payment.TaxAmount,
                Total = payment.Price,
                ExpireDate = userPlan.Expire,
                IsExpire = userPlan.Expire < CurrentCambodiaTime,
                IsRenew = userPlan.Expire != null && !userPlan.IsTrail,
                PlanId = userPlan.PlanId,
                SubscriptionId = userPlan.SubcriptionId,
                IsTax=payment.IsTax,
                IsTrail=userPlan.IsTrail,
                PaymentDescription = $"Skai Invoice {userPlan.Plan.Name} plan for 1 "+(userPlan.SubcriptionId==1?"month.":"year."),
                User = new User
                {
                    Id = userPlan.UserId,
                    Email = userPlan.User.Email,
                    Name = userPlan.User.AspNetUserClaims.FirstOrDefault(u => u.ClaimType == ClaimTypes.Name).ClaimValue,
                },
                Plans=context.Plan.Where(u=>u.ProjectPlanId==1).Select(u=>new Models.Plan
                {
                    Id=u.Id,
                    Name=u.Name,
                    MonthlyPrice=u.MonthlyPrice,
                    MonthlyRenewalPrice=u.MonthlyRenewPrice,
                    YearlyPrice=u.YearlyPrice,
                    YearlyRenewalPrice=u.YearlyRenewPrice
                }).ToList(),
                SubscriptionTypes=context.SubscriptionType.Select(u=>new Models.SubscriptionType
                {
                     Id=u.Id,
                     Name= u.Name
                }).ToList(),
                PaymentId=payment.Id
                
            };
        }

        public void RemovePromotionCode(long id)
        {
            using var context = Context();
            var payment = context.UserPayment.FirstOrDefault(u => u.Id == id);
            if (payment.IsPaid)
            {
                throw new Exception("Payment already paid.");
            }
            payment.CouponCode = null;
            payment.DiscountRate = null;
            payment.TotalAfterDiscount = payment.Price;
            payment.TaxAmount =payment.IsTax? (payment.TotalAfterDiscount * 10) / 100:0;
            payment.TotalIncludeTax = payment.TotalAfterDiscount + payment.TaxAmount;
            context.SaveChanges();
        }

        public void SaveSubscription(UserLicenseInformationDetail userLicense)
        {
            using var context = Context();
            var userPlan = context.UserPlan.FirstOrDefault(u => u.UserId == userLicense.UserId);
            if (userPlan.Expire != null && !userPlan.IsTrail)
            {
                throw new Exception("This user doesn't allow to change plan");
            }
            var userPayment = context.UserPayment.FirstOrDefault(u => u.UserId == userPlan.UserId && !u.IsPaid);
            userPlan.SubcriptionId = userLicense.SubscriptionId;
            userPlan.PlanId = userLicense.PlanId;
            var plan = context.Plan.FirstOrDefault(u => u.Id == userLicense.PlanId);
            userPlan.RenewPrice = userLicense.SubscriptionId == 2 ? plan.YearlyRenewPrice:plan.MonthlyRenewPrice;
            userPlan.Price = userPlan.SubcriptionId == 2 ? plan.YearlyPrice : plan.MonthlyPrice;
            userPayment.Price = userPlan.Price;
            userPayment.TotalAfterDiscount = userPlan.Price;
            if (userPayment.DiscountRate != null)
            {
                userPayment.TotalAfterDiscount -= (userPayment.TotalAfterDiscount * userPayment.DiscountRate.Value) / 100;
            }
            userPayment.TaxAmount =userPayment.IsTax? (userPayment.TotalAfterDiscount * 10) / 100:0;
            userPayment.TotalIncludeTax = userPayment.TotalAfterDiscount + userPayment.TaxAmount;
            context.SaveChanges();
        }
        protected string GenerateInvoiceNumber(InvoiceContext context, bool isTax,DateTime date)
        {
            var invoice = context.UserPaymentInvoice.Where(u => u.IdNavigation.IsTax == isTax && u.Year == date.Year).OrderByDescending(u => u.Date).ThenByDescending(u => u.Id)
                .FirstOrDefault();
            int number = 0;
            if (invoice != null)
            {
                number = int.Parse(invoice.Number.Split('-')[invoice.Number.Split('-').Length - 1]);
            }
            number += 1;
            return (isTax ? "" : "N") + $"{date.Year}-{date.Month:00}-{number:000000}";
        }
    }
}
