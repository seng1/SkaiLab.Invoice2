using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Models
{
    public class UserLicenseInformation
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int PlanId { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int SubscriptionId { get; set; }
        public bool IsExpire { get; set;}
        public bool IsTrail { get; set; }
        public bool IsUserHasLicense { get; set; }
        public bool IsAlertRenewLicense { get; set; }
        public bool IsRenew { get; set; }
        public string PaymentDescription { get; set; }
        public bool IsUserCompleteLicense { get; set; }
        
    }
    public class UserLicenseInformationDetail : UserLicenseInformation
    {
        public List<Plan> Plans { get; set; }
        public List<SubscriptionType> SubscriptionTypes { get; set; }
        public string CouponCode { get; set; }
        public double Total { get; set; }
        public double? DiscountRate { get; set; }
        public double TotalAfterDiscount { get; set; }
        public double TaxAmount { get; set; }
        public double TotalIncludeTax { get; set; }
        public long PaymentId { get; set; }
        public double? TotalDiscount { get; set; }
        public bool IsTax { get; set; }
    }
}
