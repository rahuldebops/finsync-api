using Dapper;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.DTO;
using finsyncapi.Dto;
using finsyncapi.Helper;
using finsyncapi.Helpers;
using finsyncapi.Models;
using Microsoft.EntityFrameworkCore;
using static finsyncapi.Helpers.Enums;
using System.Text.Json;

namespace finsyncapi.DAL.Repositories
{
    public class TransactionRepository : Repository<Transaction, DB1Context>, ITransactionRepository
    {
        private readonly DB1Context _context;
        private readonly DapperContext _dapperContext;

        public TransactionRepository(DB1Context context, DapperContext dapperContext) : base(context)
        {
            _context = context;
            _dapperContext = dapperContext;
        }


        public async Task<PagedResponse<PersonalTransactionListItemDto>> GetPersonalTransactionsListAsync(long profileId, QueryParameters query)
        {
            const string sql = @"
                SELECT 
                    t.id as transactionId,
                    t.amount,
                    t.transaction_type_id AS TransactionTypeId,
                    tt.transaction_type_name AS TransactionTypeName,
                    tp.category_id AS CategoryId,
                    c.category_name AS CategoryName,
                    t.title,
                    t.description,
                    t.transaction_date AS TransactionDate,
                    COUNT(*) OVER() AS TotalCount
                FROM txn.transactions t
                JOIN master.transaction_types tt ON t.transaction_type_id = tt.id
                LEFT JOIN LATERAL (
                    SELECT category_id 
                    FROM txn.transaction_payments 
                    WHERE transaction_id = t.id AND is_active = TRUE
                    LIMIT 1
                ) tp ON TRUE
                LEFT JOIN app.categories c ON tp.category_id = c.id AND c.is_active = TRUE
                WHERE t.profile_id = @ProfileId AND t.is_active = TRUE";

            using var connection = _dapperContext.CreateConnection();
            return await RawSqlQueryBuilder<PersonalTransactionListItemDto>.ExecuteAsync(connection, sql, query, new {ProfileId = profileId});
        }

        public async Task<PersonalTransactionViewDto?> GetPersonalTransactionByIdAsync(long transactionId, long profileId)
        {
            const string sql = @"
                SELECT 
                    t.id as transactionId,
                    t.amount,
                    t.transaction_type_id AS TransactionTypeId,
                    tt.transaction_type_name AS TransactionTypeName,
                    tp.category_id AS CategoryId,
                    c.category_name AS CategoryName,
                    t.title,
                    t.description,
                    t.transaction_date AS TransactionDate,
                    tp.account_id AS AccountId,
                    tp.debit_credit AS DebitCredit,
                    a.name AS AccountName
                FROM txn.transactions t
                JOIN master.transaction_types tt ON t.transaction_type_id = tt.id
                LEFT JOIN txn.transaction_payments tp ON t.id = tp.transaction_id AND tp.is_active = TRUE
                LEFT JOIN app.categories c ON tp.category_id = c.id AND c.is_active = TRUE
                LEFT JOIN app.accounts a ON tp.account_id = a.id AND a.is_active = TRUE
                WHERE t.id = @TransactionId AND t.profile_id = @ProfileId AND t.is_active = TRUE;";

            using var connection = _dapperContext.CreateConnection();

            var rows = (await connection.QueryAsync<TransactionPaymentRow>(sql, new { TransactionId = transactionId, ProfileId = profileId })).ToList();

            if (!rows.Any())
            {
                throw new AppException(Messages.TransactionNotFound);
            }

            var first = rows.First();

            var result = new PersonalTransactionViewDto
            {
                TransactionId = first.TransactionId,
                Amount = first.Amount,
                TransactionTypeId = first.TransactionTypeId,
                TransactionTypeName = first.TransactionTypeName,
                CategoryId = first.CategoryId ?? 0,
                CategoryName = first.CategoryName ?? string.Empty,
                Title = first.Title,
                Description = first.Description,
                TransactionDate = first.TransactionDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                Payment = new PersonalPaymentViewDto()
            };

            foreach (var row in rows)
            {
                if (row.DebitCredit == (short)DebitCreditType.Debit)
                {
                    result.Payment.DebtorAccountId = row.AccountId;
                    result.Payment.DebtorAccountName = row.AccountName;
                }
                else if (row.DebitCredit == (short)DebitCreditType.Credit || row.DebitCredit == (short)DebitCreditType.Transfer)
                {
                    result.Payment.CreditorAccountId = row.AccountId;
                    result.Payment.CreditorAccountName = row.AccountName;
                }
            }

            return result;
        }

        public async Task<ResultDto<SnowFlakeId>> AddPersonalTransactionDbAsync(string payload)
        {
            const string sql = "SELECT txn.fn_add_personal_transaction(@Payload::jsonb);";
            using var connection = _dapperContext.CreateConnection();
            var jsonResult = await connection.ExecuteScalarAsync<string>(sql, new { Payload = payload });

            var dbResult = JsonSerializer.Deserialize<DbFunctionResultDto<AddTransactionDataDto>>(jsonResult);

            if (dbResult.Success)
            {
                return new ResultDto<SnowFlakeId>
                {
                    Data = dbResult.Data.TransactionId,
                    Message = dbResult?.Message ?? "",
                    Success = dbResult?.Success ?? false
                };
            }
            throw new AppException(dbResult.Message);
            
        }
    }
}
