using RinhaDeBackend.API.Data.Models;
using System.Data;

namespace RinhaDeBackend.API.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<BankTransaction>> GetTransactionsByCustomerIdAsync(int customerId, int limit = 1000, IDbConnection? connection = null);
        Task<bool> InsertAsync(BankTransaction transaction, IDbConnection? connection = null);
    }
}
