using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string OrganisationId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool TrackInventory { get; set; }
        public ProductSalePurchaseDetail ProductPurchaseInformation { get; set; }
        public ProductSalePurchaseDetail ProductSaleInformation { get; set; }
        public List<InventoryHistory> InventoryHistories { get; set; }
        public long? LocationId { get; set; }
        public Location Location { get; set; }
        public int QtyBalance { get; set; }

    }
    public class ProductSalePurchaseDetail
    {
        public decimal Price { get; set; }
        public long? TaxId { get; set; }
        public Tax Tax { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
    }
}
