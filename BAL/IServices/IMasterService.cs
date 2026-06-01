using System.Collections.Generic;
using System.Threading.Tasks;
using finsyncapi.DAL.Entities;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.BAL.IServices
{
    public interface IMasterService
    {
        Task<PagedResponse<MasterDto>> GetAccountTypesAsync(QueryParameters queryParameters);
        Task<PagedResponse<MasterDto>> GetCurrenciesAsync(QueryParameters queryParameters);
        Task<PagedResponse<MasterDto>> GetTransactionTypesAsync(QueryParameters queryParameters);
        Task<PagedResponse<CategoryDto>> GetCategoryAsync(UserContext user, short transactionTypeId, QueryParameters queryParameters);
    }
}
