using Npgsql;

namespace RinhaDeBackend.API.Data
{
    public interface IConnectionFactory : IDisposable
    {
        Task<NpgsqlConnection> GetConnectionAsync();
    }
}