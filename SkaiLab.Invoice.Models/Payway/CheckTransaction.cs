using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Payway
{
    public class CheckTransaction
    {
        [JsonProperty("status")]
        public int StatusId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonProperty("total_amount")]
        public decimal TotalAmount { get; set; }
    }
}
