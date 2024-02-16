using RinhaDeBackend.API.Data.Models;

namespace RinhaDeBackend.API.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(int customerId);
        Task<int> UpdateBalance(int customerId);
        Task<int> UpdateBalance(int customerId, int transactionAmount);
    }
}
