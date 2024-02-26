using Npgsql;
using RinhaDeBackend.API.Data.Models;
using System.Data;
using System.Transactions;

namespace RinhaDeBackend.API.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public CustomerRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            using var connection = _connectionFactory.GetConnection();

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

        public async Task<bool> CheckIfExists(int customerId)
        {
            using var connection = _connectionFactory.GetConnection();

            await using var command = new NpgsqlCommand("SELECT customer_id FROM customers where customer_id = $1");
            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = customerId });
            command.Connection = connection;
            await using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync();
        }

        public async Task<(int? balance, int? limit)> UpdateBalance(int customerId, int transactionAmount)
        {
            using var connection = _connectionFactory.GetConnection();

            await using var command = new NpgsqlCommand(
                @"UPDATE customers
                    SET 
                        customer_balance = (customer_balance + $2) 
                    WHERE 
                        customer_id = $1 
                        AND (($2 > 0) or ((customer_balance + $2) >= (customer_limit * -1))) 
                    RETURNING customer_balance, customer_limit");

            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = customerId });
            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = transactionAmount });
            command.Connection = connection;

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var balance = reader.GetInt32("customer_balance");
                var limit = reader.GetInt32("customer_limit");

                return (balance, limit);
            }

            return (null, null);
        }
    }
}
