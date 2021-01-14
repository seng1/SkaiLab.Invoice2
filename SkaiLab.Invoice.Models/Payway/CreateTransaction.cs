using Newtonsoft.Json;

namespace SkaiLab.Invoice.Models.Payway
{
    public class CreateTransaction
    {
        [JsonProperty("tran_id")]
        public string TransactionId { get; set; }
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("firstname")]
        public string Firstname { get; set; }
        [JsonProperty("lastname")]
        public string Lastname { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("continue_success_url")]
        public string ContinueSuccessUrl { get; set; }
        [JsonProperty("Items")]
        public string Items { get; set; }
        public string ApiUrl { get; set; }
    }
}
