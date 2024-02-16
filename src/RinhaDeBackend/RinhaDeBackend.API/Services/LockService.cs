
using Npgsql;
using RinhaDeBackend.API.Data;
using RinhaDeBackend.API.Data.Models;

namespace RinhaDeBackend.API.Services
{
    public class LockService : ILockService
    {
        public async Task<bool> AcquireLockAsync(NpgsqlConnection connection, int group, int id)
        {
            await using var command = new NpgsqlCommand("SELECT pg_advisory_lock($1, $2)");
            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = group });
            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = id });
            command.Connection = connection;

            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }

        public async Task<bool> ReleaseLockAsync(NpgsqlConnection connection, int group, int id)
        {
            await using var command = new NpgsqlCommand("SELECT pg_advisory_unlock($1, $2)");
            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = group });
            command.Parameters.Add(new NpgsqlParameter<int> { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer, Value = id });
            command.Connection = connection;

            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }
    }
}
