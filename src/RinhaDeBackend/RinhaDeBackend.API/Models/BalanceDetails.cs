using System.Text.Json.Serialization;

namespace RinhaDeBackend.API.Models
{
    public class BalanceDetails
    {
        [JsonPropertyName("saldo")]
        public Balance Balance { get; set; }

        [JsonPropertyName("ultimas_transacoes")]
        public IEnumerable<TransactionInfo> LatestTransactions { get; set; }

        public BalanceDetails(Balance balance, IEnumerable<TransactionInfo> latestTransactions)
        {
            Balance = balance;
            LatestTransactions = latestTransactions;
        }
    }
}
