using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using finsyncapi.BAL.Services;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.DAL.Repositories
{
    public class AccountRepository : Repository<Account, DB1Context>, IAccountRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly ISnowflakeService _sfService;
        public AccountRepository(DB1Context context, DapperContext dapperContext, ISnowflakeService sfService) : base(context)
        {
            _dapperContext = dapperContext;
            _sfService = sfService;
        }

        public async Task<PagedResponse<AccountDto>> GetAccountsAsync(PaginationQuery pagination, long userId, long? profileId)
        {
            var sql = @"
                SELECT 
                    a.id            AS Id,
                    a.name          AS Name,
                    at.name         AS AccountTypeName,
                    a.balance       AS Balance,
                    a.currency_code AS CurrencyCode,
                    a.is_active     AS IsActive
                FROM app.accounts a
                JOIN master.account_types at ON a.account_type_id = at.id
                JOIN app.account_profiles ap ON a.id = ap.account_id AND ap.is_active = TRUE
                WHERE a.user_id = @UserId AND a.is_active = TRUE";

            if (profileId.HasValue)
            {
                sql += " AND ap.profile_id = @ProfileId";
            }

            sql += " ORDER BY a.created_at DESC;";

            using var connection = _dapperContext.CreateConnection();
            return await ExecutePagedQueryAsync<AccountDto>(connection, sql, pagination, new { UserId = userId, ProfileId = profileId });
        }

        public async Task<ResultDto<SnowFlakeId>> CreateAccountAsync(long userId, long profileId, AccountCreateDto req)
        {
            const string sql = @"
                WITH new_account AS (
                    INSERT INTO app.accounts (user_id, name, account_type_id, balance, balance_as_of, currency_code, created_by, updated_by)
                    VALUES (@UserId, @Name, @AccountTypeId, @Balance, @BalanceAsOf, @CurrencyCode, @UserId, @UserId)
                    RETURNING id
                )
                INSERT INTO app.account_profiles (account_id, profile_id, created_by, updated_by)
                SELECT id, @ProfileId, @UserId, @UserId FROM new_account
                RETURNING account_id;";

            using var connection = _dapperContext.CreateConnection();
            var accountId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                UserId = userId,
                ProfileId = profileId,
                Name = req.Name,
                AccountTypeId = req.AccountTypeId,
                Balance = req.Balance,
                BalanceAsOf = req.BalanceAsOf,
                CurrencyCode = req.CurrencyCode
            });

            return new ResultDto<SnowFlakeId> { Data = accountId, Message = "Account created successfully", Success = true };
        }

        public async Task<ResultDto<bool>> LinkAccountAsync(long userId, long profileId, long accountId)
        {
            using var connection = _dapperContext.CreateConnection();

            // 1. Verify account exists and belongs to the user
            const string verifySql = "SELECT EXISTS(SELECT 1 FROM app.accounts WHERE id = @AccountId AND user_id = @UserId AND is_active = TRUE)";
            var ownsAccount = await connection.ExecuteScalarAsync<bool>(verifySql, new { AccountId = accountId, UserId = userId });

            if (!ownsAccount)
            {
                return new ResultDto<bool> { Success = false, Message = "Account not found or unauthorized." };
            }

            // 2. Check if already linked
            const string checkLinkSql = "SELECT EXISTS(SELECT 1 FROM app.account_profiles WHERE account_id = @AccountId AND profile_id = @ProfileId AND is_active = TRUE)";
            var alreadyLinked = await connection.ExecuteScalarAsync<bool>(checkLinkSql, new { AccountId = accountId, ProfileId = profileId });

            if (alreadyLinked)
            {
                return new ResultDto<bool> { Success = false, Message = "Account is already linked to this profile." };
            }

            // 3. Link the account
            const string linkSql = @"
                INSERT INTO app.account_profiles (id, account_id, profile_id, created_by, updated_by)
                VALUES (@Id, @AccountId, @ProfileId, @UserId, @UserId);";

            await connection.ExecuteAsync(linkSql, new { Id = _sfService.NextId(), AccountId = accountId, ProfileId = profileId, UserId = userId });

            return new ResultDto<bool> { Success = true, Message = "Account linked successfully.", Data = true };
        }
    }
}
