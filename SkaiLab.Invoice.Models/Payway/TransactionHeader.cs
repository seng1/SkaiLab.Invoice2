using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models.Payway
{
    public class TransactionHeader
    {
        [JsonProperty("status")]
        public int StatusId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("Total")]
        public int total { get; set; }
        [JsonProperty("transactions")]
        public List<Transaction> Transactions { get; set; }
    }
    public class Transaction
    {
        [JsonProperty("order_id")]
        public string OrderId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("datetime")]
        public string DateString { get; set; }

        public DateTime Date
        {
            get
            {
                return DateTime.ParseExact(DateString, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            }
        }

        [JsonProperty("total_amount")]
        public double TotalAmount { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("apv")]
        public string Apv { get; set; }
        [JsonProperty("payment_type")]
        public string PaymentType { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("source_of_fund")]
        public string SourceOfFund { get; set; }
    }
}
