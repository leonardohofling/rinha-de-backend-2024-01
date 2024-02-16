using Npgsql;

namespace RinhaDeBackend.API.Services
{
    public interface ILockService
    {
        Task<bool> AcquireLockAsync(NpgsqlConnection connection, int group, int id);
        Task<bool> ReleaseLockAsync(NpgsqlConnection connection, int group, int id);
    }
}