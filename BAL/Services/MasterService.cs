using System.Collections.Generic;
using System.Threading.Tasks;
using finsyncapi.BAL.IServices;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;
using finsyncapi.Models;
using static finsyncapi.Helper.ProviderConstants;

namespace finsyncapi.BAL.Services
{
    public class MasterService : IMasterService
    {
        private readonly IMasterRepository _masterRepository;

        public MasterService(IMasterRepository masterRepository)
        {
            _masterRepository = masterRepository;
        }

        public async Task<PagedResponse<MasterDto>> GetAccountTypesAsync(QueryParameters queryParameters)
        {
            return await _masterRepository.GetAllSelectedColumnAsync<AccountType, MasterDto>(
                x => new MasterDto
                {
                    Id = x.Id,
                    Name = x.Name
                },
                queryParameters
            );
        }

        public async Task<PagedResponse<MasterDto>> GetCurrenciesAsync(QueryParameters queryParameters)
        {
            return await _masterRepository.GetAllSelectedColumnAsync<Currency, MasterDto>(
                x => new MasterDto
                {
                    Id = x.Id,
                    Name = $"{x.CurrencyCode} - {x.Name}"
                },
                queryParameters,
                x => x.IsActive
            );
        }

        public async Task<PagedResponse<MasterDto>> GetTransactionTypesAsync(QueryParameters queryParameters)
        {
            return await _masterRepository.GetAllSelectedColumnAsync<TransactionType, MasterDto>(
                x => new MasterDto
                {
                    Id = x.Id,
                    Name = x.TransactionTypeName
                },
                queryParameters
            );
        }

        public async Task<PagedResponse<CategoryDto>> GetCategoryAsync(UserContext user, short transactionTypeId, QueryParameters queryParameters)
        {
            queryParameters.FilterModel.Filters["transactionTypeId"] = new List<Condition> { new Condition { Value = transactionTypeId, MatchMode = Operators.EqualsTo} };

            return await _masterRepository.GetAllSelectedColumnAsync<Category, CategoryDto>(
                x => new CategoryDto
                {
                    Id = x.Id,
                    CategoryName = x.CategoryName,
                    TransactionTypeId = x.TransactionTypeId,
                },
                queryParameters
            );
        }
    }
}
