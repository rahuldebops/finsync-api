using finsyncapi.Dto;
using finsyncapi.DTO;
using finsyncapi.Models;

namespace finsyncapi.BAL.IServices
{
    public interface ITransactionService
    {
        Task<ResultDto<SnowFlakeId>> AddPersonalTransactionAsync(UserContext currentUser, PersonalTransactionCreateDto req);
        Task<ResultDto<SnowFlakeId>> AddPersonalTransactionDbAsync(UserContext currentUser, PersonalTransactionCreateDto req);
        Task<ResultDto<SnowFlakeId>> UpdatePersonalTransactionDbAsync(UserContext currentUser, PersonalTransactionUpdateDto req);
        Task<ResultDto<PersonalTransactionViewDto>> GetPersonalTransactionByIdAsync(UserContext currentUser, SnowFlakeId transactionId);
        Task<PagedResponse<PersonalTransactionListItemDto>> GetPersonalTransactionsListAsync(UserContext currentUser, QueryParameters pagination);
    }
}