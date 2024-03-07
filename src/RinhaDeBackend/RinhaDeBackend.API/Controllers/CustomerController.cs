using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RinhaDeBackend.API.Models;
using RinhaDeBackend.API.Services;

namespace RinhaDeBackend.API.Controllers
{
    [ApiController]
    [Route("clientes/{id}")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMemoryCache _memoryCache;
        private readonly DiagnosticsConfig _diagnosticsConfig;

        public CustomerController(ICustomerService customerService, IMemoryCache memoryCache, DiagnosticsConfig diagnosticsConfig)
        {
            _customerService = customerService;
            _memoryCache = memoryCache;
            _diagnosticsConfig = diagnosticsConfig;
        }

        [HttpGet("extrato")]
        public async Task<ActionResult<BalanceDetails>> GetBalance(int id)
        {
#if DEBUG
            using var activity = _diagnosticsConfig.Source.StartActivity("CustomerController.GetBalance()");
#endif

            if (_memoryCache.TryGetValue(string.Format(CacheConstants.CUSTOMER_EXISTS, id), out bool userExists) && !userExists)
                return NotFound();

            var serviceResult = await _customerService.GetBalanceDetailsByCustomerIdAsync(id);
            if (serviceResult.IsError)
                return HandleError(serviceResult.ErrorCode);

            return Ok(serviceResult.Result);
        }

        [HttpPost("transacoes")]
        public async Task<ActionResult<NewTransactionResponse>> PostNewTransaction(int id, [FromBody] NewTransactionRequest newTransactionRequest)
        {
#if DEBUG
            using var activity = _diagnosticsConfig.Source.StartActivity("CustomerController.PostNewTransaction()");
#endif

            if (!ModelState.IsValid)
                return UnprocessableEntity();

            if (_memoryCache.TryGetValue(string.Format(CacheConstants.CUSTOMER_EXISTS, id), out bool userExists) && !userExists)
                return NotFound();

            var serviceResult = await _customerService.NewBankTransactionAsync(id, newTransactionRequest);
            if (serviceResult.IsError)
                return HandleError(serviceResult.ErrorCode);

            return Ok(serviceResult.Result);
        }

        private ActionResult HandleError(ServiceErrorCodeEnum errorCode)
        {
            switch (errorCode)
            {
                case ServiceErrorCodeEnum.NotFound:
                    return NotFound();
                case ServiceErrorCodeEnum.InsufficientLimit:
                    return UnprocessableEntity("InsufficientLimit");
            }

            return Problem("Generic Error");
        }
    }
}
