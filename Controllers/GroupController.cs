using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using finsyncapi.Attributes;
using finsyncapi.BAL.IServices;
using finsyncapi.Dto;
using finsyncapi.Helper;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Profile(ProfileRequirement.Required)]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IClaimService _claimService;

        public GroupController(IGroupService groupService, IClaimService claimService)
        {
            _groupService = groupService;
            _claimService = claimService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] GroupCreateDto req)
        {
            var currentUser = _claimService.UserContext;
            var res = await _groupService.CreateGroupAsync(currentUser, req);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinGroup([FromBody] GroupJoinDto req)
        {
            var currentUser = _claimService.UserContext;
            var res = await _groupService.JoinGroupAsync(currentUser, req);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetGroups()
        {
            var currentUser = _claimService.UserContext;
            var res = await _groupService.GetGroupsByProfileAsync(currentUser);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }
    }
}
