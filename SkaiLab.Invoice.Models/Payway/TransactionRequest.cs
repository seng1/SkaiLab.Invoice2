using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Payway
{
    public class TransactionRequest
    {
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}
