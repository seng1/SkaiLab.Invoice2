using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Report
{
    public class ProductSaleSummary
    {
        public Product Product { get; set; }
        public decimal AvgPurchasePrice { get; set; }
        public int PurchaseQty { get; set; }
        public decimal PurchaseTotal { get; set; }
        public decimal AvgSalePrice { get; set; }
        public int SaleQty { get; set; }
        public decimal SaleTotal { get; set; }
        public int NetQty { get; set; }
        public decimal NetTotal { get; set; }
    }
}
