using System.Text.Json.Serialization;

namespace RinhaDeBackend.API.Models
{
    public class NewTransactionResponse
    {
        [JsonPropertyName("limite")]
        public int? Limit { get; set; }
        [JsonPropertyName("saldo")]
        public int? Balance { get; set; }

        public NewTransactionResponse(int? limit, int? balance)
        {
            Limit = limit;
            Balance = balance;
        }
    }
}
