using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class Option
    {
        public string ConnectionString { get; set; }
        public string JwtSecret { get; set; }
        public string NoProductImageUrl { get; set; }
        public BlobStorage BlobStorage { get; set; }
        public bool IsTax { get; set; }
        public NReco NReco { get; set; }
        public Facebook Facebook { get; set; }
        public Facebook Microsoft { get; set; }
        public string InvoiceUrl { get; set; }
        public string LandingWebsiteUrl { get; set; }
        public PayWay PayWay { get; set; }
        public MailKit MailKit { get; set; }

    }
    public class MailKit
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class PayWay
    {
        public string ApiKey { get; set; }
        public string MerchantId { get; set; }
        public string ApiUrl { get; set; }
        public string MerchantName { get; set; }
        public string BaseUrl { get; set; }
    }
    public class Facebook
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }
    public class BlobStorage
    {
        public string AccountConnection { get; set; }
        public string ContainerName { get; set; }
        public string ContainerUrl { get; set; }
    }
    public class NReco
    {
        public string Owner { get; set; }
        public string Key { get; set; }
    }

}
