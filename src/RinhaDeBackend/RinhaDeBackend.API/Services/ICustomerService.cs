using RinhaDeBackend.API.Models;

namespace RinhaDeBackend.API.Services
{
    public interface ICustomerService
    {
        Task<ServiceResult<BalanceDetails>> GetBalanceDetailsByCustomerIdAsync(int customerId);
        Task<ServiceResult<NewTransactionResponse>> NewBankTransactionAsync(int customerId, NewTransactionRequest request);
    }
}