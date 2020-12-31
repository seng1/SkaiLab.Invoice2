using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class PurchaseOrder: Expense
    {
      
    }
    
    public class PurchaseOrderItem:ExpenseItem
    {
        public long? ProductId { get; set; }
        public Product Product { get; set; }
        public long? LocationId { get; set; }
    }
    public class PurchaseOrderForUpdate
    {
        public PurchaseOrder PurchaseOrder { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<Location> Locations { get; set; }
        public List<Tax> Taxes { get; set; }
        public int BaseCurrencyId { get; set; }
        public Currency TaxCurrency { get; set; }

    }
}
