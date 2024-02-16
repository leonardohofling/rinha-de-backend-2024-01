using RinhaDeBackend.API.Models;

namespace RinhaDeBackend.API.Services
{
    public interface ICustomerService
    {
        Task<ServiceResult<BalanceDetails>> GetBalanceDetailsByCustomerId(int customerId);
        Task<ServiceResult<NewTransactionResponse>> NewBankTransaction(int customerId, NewTransactionRequest request);
    }
}