using RinhaDeBackend.API.Data.Models;

namespace RinhaDeBackend.API.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int customerId);
        Task<bool> CheckIfExists(int customerId);
        Task<(int? balance, int? limit)> UpdateBalance(int customerId, int transactionAmount);
    }
}
