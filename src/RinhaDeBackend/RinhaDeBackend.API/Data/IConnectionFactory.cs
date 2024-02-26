using Npgsql;

namespace RinhaDeBackend.API.Data
{
    public interface IConnectionFactory : IDisposable
    {
        NpgsqlConnection GetConnection();
    }
}