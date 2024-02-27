using Microsoft.Extensions.Configuration;
using Npgsql;
using RinhaDeBackend.API.Controllers;

namespace RinhaDeBackend.API.Data
{
    public sealed class ConnectionFactory : IConnectionFactory
    {
        private readonly NpgsqlDataSource _dataSource;

        public ConnectionFactory(IConfiguration configuration, ILogger<ConnectionFactory> logger)
        {
            _dataSource = NpgsqlDataSource.Create(configuration.GetConnectionString("Default")!);
        }        

        public async Task<NpgsqlConnection> GetConnectionAsync()
        {
            return await _dataSource.OpenConnectionAsync();
        }

        public void Dispose()
        {
            _dataSource?.Dispose();
        }
    }
}
