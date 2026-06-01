using System.Collections.Generic;
using System.Threading.Tasks;
using finsyncapi.BAL.IServices;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.BAL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<PagedResponse<AccountDto>> GetAccountsAsync(UserContext currentUser, PaginationQuery pagination)
        {
            return await _accountRepository.GetAccountsAsync(pagination, currentUser.UserId, currentUser.ProfileId?.Value);
        }

        public async Task<ResultDto<SnowFlakeId>> CreateAccountAsync(UserContext currentUser, AccountCreateDto req)
        {
            // ProfileId is required for creating an account to establish the link.
            if (!currentUser.ProfileId.HasValue)
            {
                throw new Helper.AppException("ProfileId is required to create an account.");
            }

            return await _accountRepository.CreateAccountAsync(currentUser.UserId, currentUser.ProfileId.Value, req);
        }

        public async Task<ResultDto<bool>> LinkAccountAsync(UserContext currentUser, LinkAccountDto req)
        {
            if (!currentUser.ProfileId.HasValue)
            {
                throw new Helper.AppException("ProfileId is required to link an account.");
            }

            return await _accountRepository.LinkAccountAsync(currentUser.UserId, currentUser.ProfileId.Value, req.AccountId);
        }
    }
}
