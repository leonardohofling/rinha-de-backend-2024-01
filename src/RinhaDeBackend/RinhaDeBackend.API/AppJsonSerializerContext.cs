using RinhaDeBackend.API.Models;
using System.Text.Json.Serialization;

namespace RinhaDeBackend.API
{
    [JsonSerializable(typeof(Balance))]
    [JsonSerializable(typeof(BalanceDetails))]
    [JsonSerializable(typeof(NewTransactionRequest))]
    [JsonSerializable(typeof(NewTransactionResponse))]
    [JsonSerializable(typeof(TransactionInfo))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
