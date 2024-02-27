using RinhaDeBackend.API.Data;
using RinhaDeBackend.API.Data.Models;
using RinhaDeBackend.API.Data.Repositories;
using RinhaDeBackend.API.Models;
using System.Diagnostics;

namespace RinhaDeBackend.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IConnectionFactory _connectionFactory;
        private readonly DiagnosticsConfig _diagnosticsConfig;

        public CustomerService(ICustomerRepository customerRepository, ITransactionRepository transactionRepository, 
            IConnectionFactory connectionFactory, DiagnosticsConfig diagnosticsConfig)
        {
            _customerRepository = customerRepository;
            _transactionRepository = transactionRepository;
            _connectionFactory = connectionFactory;
            _diagnosticsConfig = diagnosticsConfig;
        }

        public async Task<ServiceResult<BalanceDetails>> GetBalanceDetailsByCustomerId(int customerId)
        {
#if DEBUG
            using var activity = _diagnosticsConfig.Source.StartActivity("CustomerService.GetBalanceDetailsByCustomerId()");
#endif
            await using var connection = await _connectionFactory.GetConnectionAsync();

            var customer = await _customerRepository.GetByIdAsync(customerId, connection);
            if (customer == null)
                return new ServiceResult<BalanceDetails>(ServiceErrorCodeEnum.NotFound);

            var transactions = await _transactionRepository.GetTransactionsByCustomerIdAsync(customerId, 10, connection);

            return Map(customer, transactions);
        }

        public async Task<ServiceResult<NewTransactionResponse>> NewBankTransaction(int customerId, NewTransactionRequest request)
        {
#if DEBUG
            using var activity = _diagnosticsConfig.Source.StartActivity("CustomerService.NewBankTransaction()", ActivityKind.Internal);
#endif

            await using var connection = await _connectionFactory.GetConnectionAsync();

            if (!(await _customerRepository.CheckIfExistsAsync(customerId, connection)))
                return new ServiceResult<NewTransactionResponse>(ServiceErrorCodeEnum.NotFound);

            var tAmount = request.Type == "c" ? Math.Abs(request.Amount) : Math.Abs(request.Amount) * -1;

            (var balance, var limit) = await _customerRepository.UpdateBalanceAsync(customerId, tAmount, connection);
            if (balance == null || limit == null)
                return new ServiceResult<NewTransactionResponse>(ServiceErrorCodeEnum.InsufficientLimit);

            var bankTransaction = new BankTransaction(customerId, tAmount, request.Type, request.Description, DateTime.UtcNow);
            _ = await _transactionRepository.InsertAsync(bankTransaction, connection);

            return new ServiceResult<NewTransactionResponse>(new NewTransactionResponse(limit, balance));
        }

        private static ServiceResult<BalanceDetails> Map(Customer customer, IEnumerable<BankTransaction> transactions)
        {
            return new ServiceResult<BalanceDetails>(new BalanceDetails(
                new Balance(customer.Balance, customer.Limit, DateTime.UtcNow),
                transactions.Select(trn => new TransactionInfo(Math.Abs(trn.Amount), trn.Type, trn.Description, trn.CreatedAt)).ToList()
            ));
        }
    }
}