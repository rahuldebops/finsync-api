using finsyncapi.Attributes;
using finsyncapi.BAL.IServices;
using finsyncapi.DTO;
using finsyncapi.Helper;
using finsyncapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Profile(ProfileRequirement.Required)]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IClaimService _claimService;

        public TransactionController(ITransactionService transactionService, IClaimService claimService)
        {
            _transactionService = transactionService;
            _claimService = claimService;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddPersonalTransaction([FromBody] PersonalTransactionCreateDto req)
        {
            var currentUser = _claimService.UserContext;
            var res = await _transactionService.AddPersonalTransactionDbAsync(currentUser, req);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetPersonalTransactions([FromQuery] QueryParameters query)
        {
            var currentUser = _claimService.UserContext;
            var res = await _transactionService.GetPersonalTransactionsListAsync(currentUser, query);
            return Ok(ResponseHelper.Success(res));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonalTransactionByIdAsync([FromRoute] SnowFlakeId id)
        {
            var currentUser = _claimService.UserContext;
            var res = await _transactionService.GetPersonalTransactionByIdAsync(currentUser, id);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }

        
    }
}