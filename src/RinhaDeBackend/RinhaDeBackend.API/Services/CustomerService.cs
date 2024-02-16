using RinhaDeBackend.API.Data;
using RinhaDeBackend.API.Data.Models;
using RinhaDeBackend.API.Data.Repositories;
using RinhaDeBackend.API.Models;

namespace RinhaDeBackend.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILockService _lockService;
        private readonly IDatabaseSession _databaseSession;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ITransactionRepository transactionRepository, ILockService lockService,
            IDatabaseSession databaseSession, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _transactionRepository = transactionRepository;
            _lockService = lockService;
            _databaseSession = databaseSession;
            _logger = logger;
        }

        public async Task<ServiceResult<BalanceDetails>> GetBalanceDetailsByCustomerId(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                return new ServiceResult<BalanceDetails>(ServiceErrorCodeEnum.NotFound);

            var transactions = await _transactionRepository.GetByCustomerIdAsync(customerId, 10);

            return Map(customer, transactions);
        }

        public async Task<ServiceResult<NewTransactionResponse>> NewBankTransaction(int customerId, NewTransactionRequest request)
        {
            bool lockAcquired = false;
            await using var connection = _databaseSession.GetConnection();
            try
            {

                if (!(lockAcquired = await _lockService.AcquireLockAsync(connection, (int)LockGroupEnum.Customer, customerId)))
                    return new ServiceResult<NewTransactionResponse>(ServiceErrorCodeEnum.AlreadyLocked);

                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                    return new ServiceResult<NewTransactionResponse>(ServiceErrorCodeEnum.NotFound);

                var tAmount = request.Type == "c" ? Math.Abs(request.Amount) : Math.Abs(request.Amount) * -1;
                if (tAmount < 0 && tAmount + customer.Balance < -customer.Limit)
                    return new ServiceResult<NewTransactionResponse>(ServiceErrorCodeEnum.InsufficientLimit);

                var bankTransaction = new BankTransaction(customerId, tAmount, request.Type, request.Description, DateTime.UtcNow);
                _ = await _transactionRepository.InsertAsync(bankTransaction);
                customer.Balance = await _customerRepository.UpdateBalance(customerId, tAmount);

                return new ServiceResult<NewTransactionResponse>(new NewTransactionResponse(customer.Limit, customer.Balance));
            }
            finally
            {
                if (lockAcquired)
                    await _lockService.ReleaseLockAsync(connection, (int)LockGroupEnum.Customer, customerId);
            }
        }

        private ServiceResult<BalanceDetails> Map(Customer customer, IEnumerable<BankTransaction> transactions)
        {
            return new ServiceResult<BalanceDetails>(new BalanceDetails(
                new Balance(customer.Balance, customer.Limit, DateTime.UtcNow),
                transactions.Select(trn => new TransactionInfo(Math.Abs(trn.Amount), trn.Type, trn.Description, trn.CreatedAt)).ToList()
            ));
        }
    }
}
