using System.Text.Json.Serialization;

namespace RinhaDeBackend.API.Models
{
    public class TransactionInfo
    {
        [JsonPropertyName("valor")]
        public int Amount { get; set; }

        [JsonPropertyName("tipo")]
        public string Type { get; set; }

        [JsonPropertyName("descricao")]
        public string Description { get; set; }

        [JsonPropertyName("realizada_em")]
        public DateTime CreatedAt { get; set; }

        public TransactionInfo(int amount, string type, string description, DateTime createdAt)
        {
            Amount = amount;
            Type = type;
            Description = description;
            CreatedAt = createdAt;
        }
    }
}
