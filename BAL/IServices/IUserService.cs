using System.Threading.Tasks;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.BAL.IServices
{
    public interface IUserService
    {
        Task<ResultDto<SnowFlakeId>> CreateProfileAsync(UserContext currentUser, ProfileCreateDto req);

        Task<ResultDto<IEnumerable<ProfileDto>>> GetProfilesByUserAsync(UserContext currentUser);

        Task<ResultDto<ProfileDto>> GetProfileByIdAsync(UserContext currentUser, SnowFlakeId profileId);
    }
}
