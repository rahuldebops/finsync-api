using System;
using System.Threading.Tasks;
using finsyncapi.BAL.IServices;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.BAL.Services
{
    public class UserService : IUserService
    {
        private readonly IProfileRepository _profileRepo;
        private readonly ISnowflakeService _snowflakeService;

        public UserService(IProfileRepository profileRepo, ISnowflakeService snowflakeService)
        {
            _profileRepo = profileRepo;
            _snowflakeService = snowflakeService;
        }

        public async Task<ResultDto<SnowFlakeId>> CreateProfileAsync(UserContext currentUser, ProfileCreateDto req)
        {
            var profileId = _snowflakeService.NextId();
            
            var createdProfileId = await _profileRepo.CreateProfileWithAccountsAsync(currentUser.UserId, profileId, req.ProfileName, req.Description);

            return new ResultDto<SnowFlakeId>
            {
                Data = createdProfileId,
                Message = "Profile created and accounts linked successfully",
                Success = true
            };
        }

        public async Task<ResultDto<IEnumerable<ProfileDto>>> GetProfilesByUserAsync(UserContext currentUser)
        {
            return await _profileRepo.GetProfilesByUserAsync(currentUser.UserId);
        }

        public async Task<ResultDto<ProfileDto>> GetProfileByIdAsync(UserContext currentUser, SnowFlakeId profileId)
        {
            var profile = await _profileRepo.GetSingleOrDefaultWithSelectedColoumnAysnc(
                x => new ProfileDto
                {
                    ProfileId = x.Id,
                    UserId = x.UserId ?? 0,
                    ProfileName = x.Name ?? string.Empty,
                    Description = x.Description
                },
                x => x.Id == profileId && x.UserId == currentUser.UserId && x.IsActive == true
            );

            return new ResultDto<ProfileDto>
            {
                Data = profile,
                Message = profile != null ? "Profile fetched successfully" : "Profile not found",
                Success = profile != null
            };
        }
    }
}
