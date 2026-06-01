using finsyncapi.DAL.Entities;
using finsyncapi.Dto;

namespace finsyncapi.DAL.IRepositories
{
    public interface IProfileRepository : IRepository<Profile>
    {
        Task<long> CreateProfileWithAccountsAsync(long userId, long profileId, string name, string description);

        Task<ResultDto<IEnumerable<ProfileDto>>> GetProfilesByUserAsync(long userId);
    }
}
