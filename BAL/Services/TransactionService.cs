using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using finsyncapi.BAL.IServices;
using finsyncapi.DAL;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;
using finsyncapi.DTO;
using finsyncapi.Helper;
using finsyncapi.Helpers;
using finsyncapi.Models;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.BAL.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISnowflakeService _sfService;
        private readonly IUnitOfWork<DB1Context> _unitOfWork;

        public TransactionService(ITransactionRepository transactionRepository, IUnitOfWork<DB1Context> unitOfWork, ISnowflakeService sfService)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _sfService = sfService;
        }

        public async Task<ResultDto<SnowFlakeId>> AddPersonalTransactionAsync(UserContext currentUser, PersonalTransactionCreateDto req)
        {

            var a = currentUser.ToJson();
            var b = req.ToJson();

            TransactionType tt = await _transactionRepository.GetSingleWithSelectedColoumnAysnc<TransactionType, TransactionType>(x => new TransactionType { Id = x.Id, DebitCredit = x.DebitCredit }, y => y.Id == req.TransactionTypeId);
             

            if (!await _transactionRepository.ExistsAsync<Category>(x => x.Id == req.CategoryId && x.TransactionTypeId == req.TransactionTypeId )) throw new AppException(Messages.InvalidCategory);


            var debtorExists = await _transactionRepository.ExistsAsync<AccountProfile>(x => x.ProfileId == currentUser.ProfileId!.Value && x.AccountId == req.Payment.DebtorAccountId);

            var creditorExists = await _transactionRepository.ExistsAsync<AccountProfile>(x => x.ProfileId == currentUser.ProfileId!.Value && x.AccountId == req.Payment.CreditorAccountId);

            if (tt.DebitCredit == (short)DebitCreditType.Transfer ? !debtorExists && !creditorExists : tt.DebitCredit == (short)DebitCreditType.Debit ? !debtorExists :  !creditorExists)
            {
                throw new AppException(Messages.InvalidAccount);
            }

            // 4. Create Entities
            var transaction = new Transaction
            {
                Id = _sfService.NextId(),
                UserId = currentUser.UserId,
                ProfileId = currentUser.ProfileId!.Value,
                Amount = req.Amount,
                TotalAmount = req.Amount,
                TransactionTypeId = req.TransactionTypeId,
                Title = req.Title,
                Description = req.Description,
                IsActive = true,
                TransactionDate = DateTimeHelper.ParseUserTimeStringToUtc(req.TransactionDate, currentUser.TimeZone),
                CreatedBy = currentUser.UserId,
                UpdatedBy = currentUser.UserId,
            };

            // 5. Execute in Transaction
            await _unitOfWork.RunInTransactionAsync(async () =>
            {
                await _transactionRepository.AddAsync(transaction);

                // IF TRANSACTION TYPE = EXPENSE THEN DATA WILL BE ADDED TO EXPENSE SPLIT
                if(tt.Id == (short)TransactionTypeEnum.Expense)
                {
                    await _transactionRepository.AddAsync<ExpenseSplit>(new ExpenseSplit
                    {
                        Id = _sfService.NextId(),
                        TransactionId = transaction.Id,
                        UserId = currentUser.UserId,
                        ProfileId = currentUser.ProfileId!.Value,
                        OwedAmount = req.Amount,
                        IsActive = true,
                        CreatedBy = currentUser.UserId,
                        UpdatedBy = currentUser.UserId,
                    });
                }


                if(tt.DebitCredit == ((short)DebitCreditType.Transfer) || tt.DebitCredit == ((short)DebitCreditType.Debit))
                {
                    await _transactionRepository.AddAsync<TransactionPayment> (new TransactionPayment
                    {
                        Id = _sfService.NextId(),
                        TransactionId = transaction.Id,
                        UserId = currentUser.UserId,
                        ProfileId = currentUser.ProfileId!.Value,
                        AccountId = req.Payment.DebtorAccountId,
                        Amount = req.Amount,
                        DebitCredit = (short)DebitCreditType.Debit,
                        CategoryId = req.CategoryId,
                        IsActive = true,
                        CreatedBy = currentUser.UserId,
                        UpdatedBy = currentUser.UserId,
                    });                    

                    await _transactionRepository.AddAsync<Ledger>(new Ledger
                    {
                        Id = _sfService.NextId(),
                        TransactionId = transaction.Id,
                        UserId = currentUser.UserId,
                        ProfileId = currentUser.ProfileId!.Value,
                        AccountId = req.Payment.DebtorAccountId,
                        Type = (short)LedgerType.PaymentRecorded,
                        Description = $"Debit for: {req.Title}",
                        CreatedAt = DateTime.UtcNow
                    });

                }

                if (tt.DebitCredit == ((short)DebitCreditType.Transfer) || tt.DebitCredit == ((short)DebitCreditType.Credit))
                {
                    await _transactionRepository.AddAsync<TransactionPayment>(new TransactionPayment
                    {
                        Id = _sfService.NextId(),
                        TransactionId = transaction.Id,
                        UserId = currentUser.UserId,
                        ProfileId = currentUser.ProfileId!.Value,
                        AccountId = req.Payment.CreditorAccountId,
                        Amount = req.Amount,
                        IsActive = true,
                        DebitCredit = tt.DebitCredit,
                        CategoryId = req.CategoryId,
                        CreatedBy = currentUser.UserId,
                        UpdatedBy = currentUser.UserId,
                    });

                    await _transactionRepository.AddAsync<Ledger>(new Ledger
                    {
                        Id = _sfService.NextId(),
                        TransactionId = transaction.Id,
                        UserId = currentUser.UserId,
                        ProfileId = currentUser.ProfileId!.Value,
                        AccountId = req.Payment.CreditorAccountId,
                        Type = (short)LedgerType.PaymentRecorded,
                        Description = $"Credit for: {req.Title}",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            });

            return new ResultDto<SnowFlakeId> { Data = transaction.Id , Message = Messages.TransactionAdded , Success = true};
        }
        public async Task<ResultDto<SnowFlakeId>> AddPersonalTransactionDbAsync(UserContext currentUser, PersonalTransactionCreateDto req)
        {
            var payloadJson = $"{{\"userContext\": {currentUser.ToJson()}, \"payload\": {req.ToJson()}}}";
            return await _transactionRepository.AddPersonalTransactionDbAsync(payloadJson);
        }

        public async Task<ResultDto<SnowFlakeId>> UpdatePersonalTransactionDbAsync(UserContext currentUser, PersonalTransactionUpdateDto req)
        {
            var payloadJson = $"{{\"userContext\": {currentUser.ToJson()}, \"payload\": {req.ToJson()}}}";
            return await _transactionRepository.UpdatePersonalTransactionDbAsync(payloadJson);
        }

        public async Task<PagedResponse<PersonalTransactionListItemDto>> GetPersonalTransactionsListAsync(UserContext currentUser, QueryParameters query)
        {
            return await _transactionRepository.GetPersonalTransactionsListAsync(currentUser.ProfileId!.Value, query);
        }

        public async Task<ResultDto<PersonalTransactionViewDto>> GetPersonalTransactionByIdAsync(UserContext currentUser, SnowFlakeId transactionId)
        {
            var result = await _transactionRepository.GetPersonalTransactionByIdAsync(transactionId, currentUser.ProfileId!.Value);         

            return new ResultDto<PersonalTransactionViewDto> { Data = result, Message = "Transaction fetched successfully", Success = true };
        }

        
    }
}