using System.Collections.Generic;
using System.Threading.Tasks;
using finsyncapi.DAL.Entities;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.DAL.IRepositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<PagedResponse<AccountDto>> GetAccountsAsync(PaginationQuery pagination, long userId, long? profileId);
        Task<ResultDto<SnowFlakeId>> CreateAccountAsync(UserContext currentUser, long profileId, AccountCreateDto req);
        Task<ResultDto<bool>> LinkAccountAsync(long userId, long profileId, long accountId);
    }
}
