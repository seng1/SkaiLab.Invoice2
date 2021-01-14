using SkaiLab.Invoice.Models.Payway;


namespace SkaiLab.Invoice.Models
{
    public class PaymentCheckout
    {
        public UserLicenseInformationDetail UserLicenseInformationDetail { get; set; }
        public CreateTransaction PayWayCreateTransaction { get; set; }
    }
}
