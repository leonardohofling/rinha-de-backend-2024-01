using Microsoft.Extensions.Configuration;
using Npgsql;
using RinhaDeBackend.API.Controllers;
using System.Diagnostics;

namespace RinhaDeBackend.API.Data
{
    public sealed class ConnectionFactory : IConnectionFactory
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly DiagnosticsConfig _diagnosticsConfig;

        public ConnectionFactory(IConfiguration configuration, DiagnosticsConfig diagnosticsConfig)
        {
            _dataSource = NpgsqlDataSource.Create(configuration.GetConnectionString("Default")!);
            _diagnosticsConfig = diagnosticsConfig;
        }        

        public async Task<NpgsqlConnection> GetConnectionAsync()
        {
            using var activity = _diagnosticsConfig.Source.StartActivity("ConnectionFactory.GetConnectionAsync()", ActivityKind.Internal);
            return await _dataSource.OpenConnectionAsync();
        }

        public void Dispose()
        {
            _dataSource?.Dispose();
        }
    }
}
