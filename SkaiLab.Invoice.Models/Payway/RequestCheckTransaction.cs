using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Payway
{
    public class RequestCheckTransaction
    {
        [JsonProperty("tran_id")]
        public string TransactionId { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}
