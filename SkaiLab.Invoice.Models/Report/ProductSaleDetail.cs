using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Models.Report
{
    public class ProductSaleDetail
    {
        public List<ProductSaleDetailItem> PurchaseItems { get; set; }
        public List<ProductSaleDetailItem> SaleItems { get; set; }
    }
    public class ProductSaleDetailItem
    {
        public DateTime Date { get; set; }
        public string To { get; set; }
        public int Qty { get; set; }
        public double? DiscountRate { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal UnitPrice { get; set; }
        public string RefNo { get; set; }
        public decimal Total
        {
            get
            {
                var total = Qty * UnitPrice;
                if (DiscountRate != null)
                {
                    total -= (total * (decimal)DiscountRate.Value) / 100;
                }
                if (TaxRate != null)
                {
                    total += (total * TaxRate.Value) / 100;
                }
                return total;
            }
        }
    }
}
