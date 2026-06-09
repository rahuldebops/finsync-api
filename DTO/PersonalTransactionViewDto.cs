using finsyncapi.Models;

namespace finsyncapi.DTO
{
    public class PersonalTransactionViewDto
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public short TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; } = string.Empty;
        public SnowFlakeId CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TransactionDate { get; set; } = string.Empty;
        public PersonalPaymentViewDto Payment { get; set; } = new();
    }
    public class PersonalPaymentViewDto
    {
        public SnowFlakeId? DebtorAccountId { get; set; }
        public string? DebtorAccountName { get; set; }
        public SnowFlakeId? CreditorAccountId { get; set; }
        public string? CreditorAccountName { get; set; }
    }
    public sealed class TransactionPaymentRow
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }

        public short TransactionTypeId { get; set; }

        public string TransactionTypeName { get; set; } = string.Empty;

        public long? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime TransactionDate { get; set; }

        public long? AccountId { get; set; }

        public short? DebitCredit { get; set; }

        public string? AccountName { get; set; }
    }

    public class PersonalTransactionListItemDto
    {
        public SnowFlakeId TransactionId { get; set; }
        public decimal Amount { get; set; }
        public short TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; } = string.Empty;
        public SnowFlakeId CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TransactionDate { get; set; } = string.Empty;
    }
    

    
}
