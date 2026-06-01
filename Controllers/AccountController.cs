using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using finsyncapi.Attributes;
using finsyncapi.BAL.IServices;
using finsyncapi.Helper;
using static finsyncapi.Helpers.Enums;
using finsyncapi.Models;
using finsyncapi.Dto;

namespace finsyncapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Profile(ProfileRequirement.None)]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IClaimService _claimService;

        public AccountController(IAccountService accountService, IClaimService claimService)
        {
            _accountService = accountService;
            _claimService = claimService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAccounts([FromQuery] PaginationQuery pagination)
        {
            var currentUser = _claimService.UserContext;
            var res = await _accountService.GetAccountsAsync(currentUser, pagination);
            return Ok(ResponseHelper.Success(res));
        }

        [HttpPost("create")]
        [Profile(ProfileRequirement.Required)]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateDto req)
        {
            var currentUser = _claimService.UserContext;
            var res = await _accountService.CreateAccountAsync(currentUser, req);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }

        [HttpPost("link")]
        [Profile(ProfileRequirement.Required)]
        public async Task<IActionResult> LinkAccount([FromBody] LinkAccountDto req)
        {
            var currentUser = _claimService.UserContext;
            var res = await _accountService.LinkAccountAsync(currentUser, req);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }
    }
}
