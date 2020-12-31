using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ProductPurchaseInformation
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public long? TaxId { get; set; }
        public string Title { get; set; }

        public virtual Product IdNavigation { get; set; }
        public virtual Tax Tax { get; set; }
    }
}
