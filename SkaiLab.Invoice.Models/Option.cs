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
        public NReco NReco { get; set; }
        public Facebook Facebook { get; set; }
        public Facebook Microsoft { get; set; }
        public string InvoiceUrl { get; set; }

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
