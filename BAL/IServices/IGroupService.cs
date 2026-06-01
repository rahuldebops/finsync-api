using System.Collections.Generic;
using System.Threading.Tasks;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.BAL.IServices
{
    public interface IGroupService
    {
        Task<ResultDto<SnowFlakeId>> CreateGroupAsync(UserContext currentUser, GroupCreateDto req);
        Task<ResultDto<bool>> JoinGroupAsync(UserContext currentUser, GroupJoinDto req);
        Task<ResultDto<IEnumerable<GroupDto>>> GetGroupsByProfileAsync(UserContext currentUser);
    }
}
