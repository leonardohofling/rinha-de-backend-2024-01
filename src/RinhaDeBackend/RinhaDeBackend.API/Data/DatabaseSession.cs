using Microsoft.Extensions.Configuration;
using Npgsql;
using RinhaDeBackend.API.Controllers;

namespace RinhaDeBackend.API.Data
{
    public sealed class DatabaseSession : IDatabaseSession
    {
        private readonly NpgsqlDataSource _dataSource;

        public DatabaseSession(IConfiguration configuration, ILogger<DatabaseSession> logger)
        {
            _dataSource = NpgsqlDataSource.Create(configuration.GetConnectionString("Default")!);
        }        

        public NpgsqlConnection GetConnection()
        {
            return _dataSource.OpenConnection();
        }

        public void Dispose()
        {
            _dataSource?.Dispose();
        }
    }
}
