using Npgsql;
using RinhaDeBackend.API.Data.Models;
using System.Data;

namespace RinhaDeBackend.API.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly DiagnosticsConfig _diagnosticsConfig;

        #region SQL Commands

        private readonly NpgsqlCommand selectCommand =
            new("SELECT customer_name, customer_limit, customer_balance FROM customers where customer_id = $1")
            {
                Parameters = { new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer } }
            };

        private readonly NpgsqlCommand checkIfExistsCommand =
            new("SELECT customer_id FROM customers where customer_id = $1")
            {
                Parameters = { new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer } }
            };

        private readonly NpgsqlCommand updateBalanceCommand = new NpgsqlCommand(
                @"UPDATE customers
                    SET 
                        customer_balance = (customer_balance + $2) 
                    WHERE 
                        customer_id = $1 
                        AND (($2 > 0) or ((customer_balance + $2) >= (customer_limit * -1))) 
                    RETURNING customer_balance, customer_limit")
        {
            Parameters = {
                new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer },
                new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer }
            }
        };

        #endregion

        public CustomerRepository(IConnectionFactory connectionFactory, DiagnosticsConfig diagnosticsConfig)
        {
            _connectionFactory = connectionFactory;
            _diagnosticsConfig = diagnosticsConfig;
        }

        public async Task<Customer?> GetByIdAsync(int customerId, IDbConnection? connection = null)
        {
#if DEBUG
            using var activity = _diagnosticsConfig.Source.StartActivity("CustomerRepository.GetByIdAsync()");
#endif

            await using var command = selectCommand.Clone();
            command.Parameters[0].Value = customerId;

            if (connection == null)
            {
                await using var newConnection = await _connectionFactory.GetConnectionAsync();
                command.Connection = newConnection;
            }
            else
                command.Connection = (NpgsqlConnection)connection;

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var customer = new Customer
                {
                    Id = customerId,
                    Name = reader.GetString(0),
                    Limit = reader.GetInt32(1),
                    Balance = reader.GetInt32(2),
                };

                return customer;
            }

            return null;
        }

        public async Task<bool> CheckIfExistsAsync(int customerId, IDbConnection? connection = null)
        {
#if DEBUG
            using var activity = _diagnosticsConfig.Source.StartActivity("CustomerRepository.CheckIfExists()");
#endif

            await using var command = checkIfExistsCommand.Clone();
            command.Parameters[0].Value = customerId;

            if (connection == null)
            {
                await using var newConnection = await _connectionFactory.GetConnectionAsync();
                command.Connection = newConnection;
            }
            else
                command.Connection = (NpgsqlConnection)connection;

            var resultObject = await command.ExecuteScalarAsync();
            return resultObject != null;
        }

        public async Task<(int? balance, int? limit)> UpdateBalanceAsync(int customerId, int transactionAmount, IDbConnection? connection = null)
        {
#if DEBUG
            using var activity = _diagnosticsConfig.Source.StartActivity("CustomerRepository.UpdateBalance()");
#endif

            await using var command = updateBalanceCommand.Clone();

            command.Parameters[0].Value = customerId;
            command.Parameters[1].Value = transactionAmount;

            if (connection == null)
            {
                await using var newConnection = await _connectionFactory.GetConnectionAsync();
                command.Connection = newConnection;
            }
            else
                command.Connection = (NpgsqlConnection)connection;

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var balance = reader.GetInt32(0);
                var limit = reader.GetInt32(1);

                return (balance, limit);
            }

            return (null, null);
        }
    }
}
