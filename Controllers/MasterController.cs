using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using finsyncapi.BAL.IServices;
using finsyncapi.Helper;
using finsyncapi.Models;
using finsyncapi.BAL.Services;

namespace finsyncapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MasterController : ControllerBase
    {
        private readonly IMasterService _masterService;
        private readonly IClaimService _claimService;

        public MasterController(IMasterService masterService, IClaimService claimService)
        {
            _masterService = masterService;
            _claimService = claimService;

        }

        [HttpGet("account-types")]
        public async Task<IActionResult> GetAccountTypes([FromQuery] QueryParameters queryParameters)
        {
            var res = await _masterService.GetAccountTypesAsync(queryParameters);
            return Ok(ResponseHelper.Success(res, "Account types fetched successfully"));
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> GetCurrencies([FromQuery] QueryParameters queryParameters)
        {
            var res = await _masterService.GetCurrenciesAsync(queryParameters);
            return Ok(ResponseHelper.Success(res, "Currencies fetched successfully"));
        }

        [HttpGet("transaction-types")]
        public async Task<IActionResult> GetTransactionTypes([FromQuery] QueryParameters queryParameters)
        {
            var res = await _masterService.GetTransactionTypesAsync(queryParameters);
            return Ok(ResponseHelper.Success(res, "Transaction types fetched successfully"));
        }
        
        [HttpGet("categories")]
        public async Task<IActionResult> GetTransactionTypes([FromQuery] short transactionTypeId, [FromQuery] QueryParameters queryParameters)
        {
            var res = await _masterService.GetCategoryAsync(_claimService.UserContext, transactionTypeId, queryParameters);
            return Ok(ResponseHelper.Success(res, "Transaction types fetched successfully"));
        }
    }
}
