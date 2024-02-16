using Npgsql;
using RinhaDeBackend.API.Data.Models;
using System.Data;

namespace RinhaDeBackend.API.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDatabaseSession _dataBaseSession;

        public TransactionRepository(IDatabaseSession databaseSession)
        {
            _dataBaseSession = databaseSession;
        }

        public async Task<IEnumerable<BankTransaction>> GetByCustomerIdAsync(int customerId, int limit = 1000)
        {
            await using var connection = _dataBaseSession.GetConnection();
            await using var command = new NpgsqlCommand($"SELECT transaction_id, transaction_amount, transaction_type, transaction_description, created_at FROM transactions where customer_id = $1 ORDER BY transaction_id DESC LIMIT {limit}");

            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = customerId });
            command.Connection = connection;

            await using var reader = await command.ExecuteReaderAsync();

            var transactions = new List<BankTransaction>();
            while (await reader.ReadAsync())
            {
                var bankTransaction = new BankTransaction
                {
                    Id = reader.GetInt32("transaction_id"),
                    Amount = reader.GetInt32("transaction_amount"),
                    Type = reader.GetString("transaction_type"),
                    Description = reader.GetString("transaction_description"),
                    CreatedAt = reader.GetDateTime("created_at")
                };

                transactions.Add(bankTransaction);
            }

            return transactions;
        }

        public async Task<bool> InsertAsync(BankTransaction transaction)
        {
            await using var connection = _dataBaseSession.GetConnection();
            await using var command = new NpgsqlCommand("INSERT INTO transactions (customer_id, transaction_amount, transaction_type, transaction_description) values($1, $2, $3, $4)");
            command.Connection = connection;

            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = transaction.CustomerId });
            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = transaction.Amount });
            command.Parameters.Add(new NpgsqlParameter<string> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Char, Value = transaction.Type });
            command.Parameters.Add(new NpgsqlParameter<string> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Char, Value = transaction.Description });
            
            var rows = await command.ExecuteNonQueryAsync();

            return rows > 0;
        }
    }
}
