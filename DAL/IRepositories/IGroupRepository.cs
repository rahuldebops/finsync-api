using System.Threading.Tasks;
using finsyncapi.DAL.Entities;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.DAL.IRepositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<ResultDto<SnowFlakeId>> CreateGroupAsync(long userId, long profileId, GroupCreateDto req);
        Task<ResultDto<bool>> JoinGroupAsync(long userId, long profileId, GroupJoinDto req);
        Task<ResultDto<IEnumerable<GroupDto>>> GetGroupsByProfileAsync(long profileId);
    }
}
