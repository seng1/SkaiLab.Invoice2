using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class UserPayment
    {
        public UserPayment()
        {
            PaywayTransactionLog = new HashSet<PaywayTransactionLog>();
        }

        public long Id { get; set; }
        public string UserId { get; set; }
        public int SubscriptionTypeId { get; set; }
        public int PlanId { get; set; }
        public double Price { get; set; }
        public string CouponCode { get; set; }
        public bool IsPaid { get; set; }
        public DateTime Created { get; set; }
        public double? DiscountRate { get; set; }
        public double TotalAfterDiscount { get; set; }
        public double TaxAmount { get; set; }
        public double TotalIncludeTax { get; set; }
        public string Description { get; set; }
        public bool IsTax { get; set; }

        public virtual Coupon CouponCodeNavigation { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual SubscriptionType SubscriptionType { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual UserPaymentInvoice UserPaymentInvoice { get; set; }
        public virtual UserPaymentPayWayDetail UserPaymentPayWayDetail { get; set; }
        public virtual UserPaymentPayway UserPaymentPayway { get; set; }
        public virtual ICollection<PaywayTransactionLog> PaywayTransactionLog { get; set; }
    }
}
