using Npgsql;
using RinhaDeBackend.API.Data.Models;
using System.Data;
using System.Transactions;

namespace RinhaDeBackend.API.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDatabaseSession _dataBaseSession;

        public CustomerRepository(IDatabaseSession databaseSession)
        {
            _dataBaseSession = databaseSession;
        }

        public async Task<Customer> GetByIdAsync(int customerId)
        {
            await using var connection = _dataBaseSession.GetConnection();

            await using var command = new NpgsqlCommand("SELECT customer_name, customer_limit, customer_balance FROM customers where customer_id = $1");
            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = customerId });
            command.Connection = connection;
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var customer = new Customer
                {
                    Id = customerId,
                    Name = reader.GetString("customer_name"),
                    Balance = reader.GetInt32("customer_balance"),
                    Limit = reader.GetInt32("customer_limit")
                };

                return customer;
            }

            return null;
        }

        public async Task<int> UpdateBalance(int customerId)
        {
            await using var connection = _dataBaseSession.GetConnection();

            await using var command = new NpgsqlCommand(
                "UPDATE customers SET customer_balance = (SELECT SUM(transaction_amount) FROM transactions WHERE customer_id = $1) WHERE customer_id = $1 RETURNING customer_balance");

            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = customerId });
            command.Connection = connection;

            var returningValue = await command.ExecuteScalarAsync();
            if (returningValue != null)
                return Convert.ToInt32(returningValue);


            throw new InvalidOperationException("Error reading data from Database");
        }

        public async Task<int> UpdateBalance(int customerId, int transactionAmount)
        {
            await using var connection = _dataBaseSession.GetConnection();

            await using var command = new NpgsqlCommand(
                "UPDATE customers SET customer_balance = customer_balance + $2 WHERE customer_id = $1 RETURNING customer_balance");

            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = customerId });
            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = transactionAmount });
            command.Connection = connection;

            var returningValue = await command.ExecuteScalarAsync();
            if (returningValue != null)
                return Convert.ToInt32(returningValue);


            throw new InvalidOperationException("Error reading data from Database");
        }
    }
}
