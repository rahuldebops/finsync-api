using System.Threading.Tasks;
using Dapper;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;

namespace finsyncapi.DAL.Repositories
{
    public class ProfileRepository : Repository<Profile, DB1Context>, IProfileRepository
    {
        private readonly DapperContext _dapperContext;

        public ProfileRepository(DB1Context context, DapperContext dapperContext) : base(context)
        {
            _dapperContext = dapperContext;
        }

        public async Task<long> CreateProfileWithAccountsAsync(long userId, long profileId, string name, string description)
        {
            const string sql = @"
                    INSERT INTO auth.profiles (id, user_id, name, description, created_by, updated_by)
                    VALUES (@ProfileId, @UserId, @Name, @Description, @UserId, @UserId)
                    RETURNING id;";

            using var connection = _dapperContext.CreateConnection();
            return await connection.ExecuteScalarAsync<long>(sql, new
            {
                ProfileId = profileId,
                UserId = userId,
                Name = name,
                Description = description
            });
        }

        public async Task<ResultDto<IEnumerable<ProfileDto>>> GetProfilesByUserAsync(long userId)
        {
            const string sql = @"
                SELECT 
                    id AS ProfileId, 
                    user_id AS UserId, 
                    name as ProfileName
                FROM auth.profiles
                WHERE user_id = @UserId AND is_active = TRUE;";

            using var connection = _dapperContext.CreateConnection();

            var profilesData = await connection.QueryAsync<ProfileDto>(sql, new { UserId = userId });

            return new ResultDto<IEnumerable<ProfileDto>>
            {
                Data = profilesData,
                Message = "Profiles fetched successfully",
                Success = true
            };
        }
    }
}
