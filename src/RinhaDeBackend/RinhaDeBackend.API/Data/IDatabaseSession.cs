using Npgsql;

namespace RinhaDeBackend.API.Data
{
    public interface IDatabaseSession : IDisposable
    {
        NpgsqlConnection GetConnection();
    }
}