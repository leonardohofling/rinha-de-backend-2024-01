using RinhaDeBackend.API.Data.Models;
using System.Data;

namespace RinhaDeBackend.API.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int customerId, IDbConnection? connection = null);
        Task<bool> CheckIfExistsAsync(int customerId, IDbConnection? connection = null);
        Task<(int? balance, int? limit)> UpdateBalanceAsync(int customerId, int transactionAmount, IDbConnection? connection = null);
    }
}
