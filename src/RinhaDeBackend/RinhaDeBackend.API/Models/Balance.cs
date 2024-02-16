using System.Text.Json.Serialization;

namespace RinhaDeBackend.API.Models
{
    public class Balance
    {
        [JsonPropertyName("total")]
        public int Amount { get; set; }

        [JsonPropertyName("limite")]
        public int Limit { get; set; }

        [JsonPropertyName("data_extrato")]
        public DateTime CreatedAt { get; set; }

        public Balance(int amount, int limit, DateTime createdAt)
        {
            Amount = amount;
            Limit = limit;
            CreatedAt = createdAt;
        }
    }
}
