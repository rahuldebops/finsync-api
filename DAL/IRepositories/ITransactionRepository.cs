using finsyncapi.DAL.Entities;
using finsyncapi.DTO;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.DAL.IRepositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<PersonalTransactionViewDto?> GetPersonalTransactionByIdAsync(long transactionId, long profileId);
        Task<PagedResponse<PersonalTransactionListItemDto>> GetPersonalTransactionsListAsync(long profileId, QueryParameters query);
        Task<ResultDto<SnowFlakeId>> AddPersonalTransactionDbAsync(string payload);
        Task<ResultDto<SnowFlakeId>> UpdatePersonalTransactionDbAsync(string payload);
    }
}