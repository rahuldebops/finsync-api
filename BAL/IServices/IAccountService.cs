using System.Collections.Generic;
using System.Threading.Tasks;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.BAL.IServices
{
    public interface IAccountService
    {
        Task<PagedResponse<AccountDto>> GetAccountsAsync(UserContext currentUser, PaginationQuery pagination);
        Task<ResultDto<SnowFlakeId>> CreateAccountAsync(UserContext currentUser, AccountCreateDto req);
        Task<ResultDto<bool>> LinkAccountAsync(UserContext currentUser, LinkAccountDto req);
    }
}
