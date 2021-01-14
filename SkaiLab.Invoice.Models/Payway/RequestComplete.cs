using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Payway
{
    public class RequestComplete
    {
        [JsonProperty("tran_id")]
        public string TransactionId { get; set; }
        [JsonProperty("status")]
        public int StatusId { get; set; }
    }
}
