using Microsoft.AspNetCore.Mvc;
using RinhaDeBackend.API.Models;
using RinhaDeBackend.API.Services;

namespace RinhaDeBackend.API.Controllers
{
    [ApiController]
    [Route("clientes/{id}")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpGet("extrato")]
        public async Task<ActionResult<BalanceDetails>> GetBalance(int id)
        {
            var serviceResult = await _customerService.GetBalanceDetailsByCustomerId(id);
            if (serviceResult.IsError)
                return HandleError(serviceResult.ErrorCode);

            return Ok(serviceResult.Result);
        }

        [HttpPost("transacoes")]
        public async Task<ActionResult<NewTransactionResponse>> PostNewTransaction(int id, [FromBody] NewTransactionRequest newTransactionRequest)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity();

            var serviceResult = await _customerService.NewBankTransaction(id, newTransactionRequest);
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
