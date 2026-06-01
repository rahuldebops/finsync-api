using System.Threading.Tasks;
using Dapper;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;
using finsyncapi.Helper;
using finsyncapi.Helpers;
using finsyncapi.Models;

namespace finsyncapi.DAL.Repositories
{
    public class GroupRepository : Repository<Group, DB1Context>, IGroupRepository
    {
        private class JoinGroupValidationResult
        {
            public bool GroupExists { get; set; }
            public bool AlreadyMember { get; set; }
        }

        private readonly DapperContext _dapperContext;

        public GroupRepository(DB1Context context, DapperContext dapperContext) : base(context)
        {
            _dapperContext = dapperContext;
        }

        public async Task<ResultDto<SnowFlakeId>> CreateGroupAsync(long userId, long profileId, GroupCreateDto req)
        {
            const string sql = @"
                WITH new_group AS (
                    INSERT INTO app.groups (group_name, created_by, updated_by)
                    VALUES (@GroupName, @CreatedBy, @CreatedBy)
                    RETURNING id
                )
                INSERT INTO app.group_profiles (group_id, profile_id, group_role, created_by, updated_by)
                SELECT id, @ProfileId, @AdminGroupRole, @CreatedBy, @CreatedBy FROM new_group
                RETURNING group_id;";

            using var connection = _dapperContext.CreateConnection();

            var groupId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                GroupName = req.GroupName,
                ProfileId = profileId,
                AdminGroupRole = Enums.GroupRoleType.ADMIN,
                CreatedBy = userId
            });

            return new ResultDto<SnowFlakeId> { Data = groupId, Message = "Group Created Successfully", Success = true };
        }

        public async Task<ResultDto<bool>> JoinGroupAsync(long userId, long profileId, GroupJoinDto req)
        {
            using var connection = _dapperContext.CreateConnection();

            // Profile ownership is already verified by ProfileAuthorizationFilter.
            // Only check group existence and duplicate membership.
            var validationQuery = @"
                SELECT
                    (SELECT EXISTS(SELECT 1 FROM app.groups WHERE id = @GroupId)) AS GroupExists,
                    (SELECT EXISTS(SELECT 1 FROM app.group_profiles WHERE group_id = @GroupId AND profile_id = @ProfileId)) AS AlreadyMember;";

            var validation = await connection.QueryFirstOrDefaultAsync<JoinGroupValidationResult>(
                validationQuery, new { GroupId = req.GroupId!.Value.Value, ProfileId = profileId });

            if (validation == null)
            {
                throw new AppException("Validation failed.");
            }

            if (!validation.GroupExists)
            {
                throw new AppException("Group does not exist.");
            }

            if (validation.AlreadyMember)
            {
                throw new AppException("User is already in the group.");
            }

            var sql = @"
                INSERT INTO app.group_profiles (group_id, profile_id, group_role, created_by, updated_by)
                VALUES (@GroupId, @ProfileId, @MemberGroupRole, @CreatedBy, @CreatedBy);";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                GroupId = req.GroupId!.Value.Value,
                ProfileId = profileId,
                MemberGroupRole = Enums.GroupRoleType.MEMBER,
                CreatedBy = userId
            });

            return new ResultDto<bool> { Data = rowsAffected > 0, Message = "Joined group successfully", Success = rowsAffected > 0 };
        }

        public async Task<ResultDto<IEnumerable<GroupDto>>> GetGroupsByProfileAsync(long profileId)
        {
            const string sql = @"
                SELECT 
                    g.id as Id, 
                    g.group_name as GroupName, 
                    gp.group_role as GroupRole 
                FROM app.groups g 
                JOIN app.group_profiles gp ON g.id = gp.group_id 
                WHERE gp.profile_id = @ProfileId 
                AND g.is_active = TRUE 
                AND gp.is_active = TRUE;";

            using var connection = _dapperContext.CreateConnection();
            var groups = await connection.QueryAsync<GroupDto>(sql, new { ProfileId = profileId });

            return new ResultDto<IEnumerable<GroupDto>> { Data = groups, Message = "Groups Fetched Successfully", Success = true };
        }
    }
}
