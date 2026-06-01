using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using finsyncapi.BAL.IServices;
using finsyncapi.Dto;
using finsyncapi.Helper;
using finsyncapi.BAL.Services;
using finsyncapi.Models;

namespace finsyncapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IClaimService _claimService;

        public UserController(IUserService userService, IClaimService claimService)
        {
            _userService = userService;
            _claimService = claimService;
        }

        [HttpPost("profile/create")]
        public async Task<IActionResult> CreateProfile([FromBody] ProfileCreateDto req)
        {
            var currentUser = _claimService.UserContext;
            var res = await _userService.CreateProfileAsync(currentUser, req);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }

        [HttpGet("profiles")]
        public async Task<IActionResult> GetProfiles()
        {
            var currentUser = _claimService.UserContext;
            var res = await _userService.GetProfilesByUserAsync(currentUser);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }

        [HttpGet("profiles/{profileId}")]
        public async Task<IActionResult> GetProfileById(SnowFlakeId profileId)
        {
            var currentUser = _claimService.UserContext;
            var res = await _userService.GetProfileByIdAsync(currentUser, profileId);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }
    }
}
