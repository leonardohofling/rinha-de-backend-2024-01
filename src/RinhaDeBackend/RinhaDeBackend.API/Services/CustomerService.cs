using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _memoryCache;
        private readonly DiagnosticsConfig _diagnosticsConfig;

        public CustomerService(ICustomerRepository customerRepository, ITransactionRepository transactionRepository,
            IConnectionFactory connectionFactory, IMemoryCache memoryCache, DiagnosticsConfig diagnosticsConfig)
        {
            _customerRepository = customerRepository;
            _transactionRepository = transactionRepository;
            _connectionFactory = connectionFactory;
            _memoryCache = memoryCache;
            _diagnosticsConfig = diagnosticsConfig;
        }

        public async Task<ServiceResult<BalanceDetails>> GetBalanceDetailsByCustomerIdAsync(int customerId)
        {
#if DEBUG
            using var activity = _diagnosticsConfig.Source.StartActivity("CustomerService.GetBalanceDetailsByCustomerId()");
#endif
            await using var connection = await _connectionFactory.GetConnectionAsync();

            var customer = await _customerRepository.GetByIdAsync(customerId, connection);
            if (customer == null)
            {
                _memoryCache.Set(string.Format(CacheConstants.CUSTOMER_EXISTS, customerId), false, DateTimeOffset.UtcNow.AddMinutes(CacheConstants.CUSTOMER_EXISTS_EXPIRATION_MINUTES));
                return new ServiceResult<BalanceDetails>(ServiceErrorCodeEnum.NotFound);
            }

            _memoryCache.Set(string.Format(CacheConstants.CUSTOMER_EXISTS, customerId), true, DateTimeOffset.UtcNow.AddMinutes(CacheConstants.CUSTOMER_EXISTS_EXPIRATION_MINUTES));

            var transactions = await _transactionRepository.GetTransactionsByCustomerIdAsync(customerId, 10, connection);

            return Map(customer, transactions);
        }

        public async Task<ServiceResult<NewTransactionResponse>> NewBankTransactionAsync(int customerId, NewTransactionRequest request)
        {
#if DEBUG
            using var activity = _diagnosticsConfig.Source.StartActivity("CustomerService.NewBankTransaction()", ActivityKind.Internal);
#endif
            bool cached;
            if ((cached = _memoryCache.TryGetValue(string.Format(CacheConstants.CUSTOMER_EXISTS, customerId), out bool userExists)) && !userExists)
                return new ServiceResult<NewTransactionResponse>(ServiceErrorCodeEnum.NotFound);

            await using var connection = await _connectionFactory.GetConnectionAsync();

            if (!cached && !(await _customerRepository.CheckIfExistsAsync(customerId, connection)))
            {
                _memoryCache.Set(string.Format(CacheConstants.CUSTOMER_EXISTS, customerId), false, DateTimeOffset.UtcNow.AddMinutes(CacheConstants.CUSTOMER_EXISTS_EXPIRATION_MINUTES));
                return new ServiceResult<NewTransactionResponse>(ServiceErrorCodeEnum.NotFound);
            }

            _memoryCache.Set(string.Format(CacheConstants.CUSTOMER_EXISTS, customerId), true, DateTimeOffset.UtcNow.AddMinutes(CacheConstants.CUSTOMER_EXISTS_EXPIRATION_MINUTES));

            (var balance, var limit) = await _customerRepository.UpdateBalanceAsync(customerId, request.GetAmountForBalance(), connection);
            if (balance == null || limit == null)
                return new ServiceResult<NewTransactionResponse>(ServiceErrorCodeEnum.InsufficientLimit);

            var bankTransaction = new BankTransaction(customerId, request.GetAmountForBalance(), request.Type, request.Description, DateTime.UtcNow);
            if (!(await _transactionRepository.InsertAsync(bankTransaction, connection)))
                return new ServiceResult<NewTransactionResponse>(ServiceErrorCodeEnum.GenericFailure);

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