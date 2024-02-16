using RinhaDeBackend.API.Data.Models;

namespace RinhaDeBackend.API.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<BankTransaction>> GetByCustomerIdAsync(int customerId, int limit = 1000);
        Task<bool> InsertAsync(BankTransaction transaction);
    }
}
