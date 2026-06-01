using finsyncapi.Helpers;
using finsyncapi.Models;
using System.ComponentModel.DataAnnotations;
using static finsyncapi.Helpers.ValidationAttributes;

namespace finsyncapi.DTO
{
    public class PaymentDto
    {
        [Required]
        public SnowFlakeId DebtorAccountId { get; set; }

        [Required]
        public SnowFlakeId CreditorAccountId { get; set; }
    }

    public class ExpenseSplitDto
    {
        [Required]
        public SnowFlakeId ProfileId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Owed amount must be greater than zero.")]
        public decimal OwedAmount { get; set; }
    }

    public class PersonalTransactionCreateDto
    {
        [Required]
        [Range(0.01, 9999999999.99, ErrorMessage = "Amount must be between 0.01 and 9999999999.99")]
        public decimal Amount { get; set; }

        [Required]
        public short TransactionTypeId { get; set; }

        public SnowFlakeId CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Iso8601Date]
        public required string TransactionDate { get; set; }

        [Required]
        public required PaymentDto Payment { get; set; }
    }

    public class GroupTransactionCreateDto
    {
        [Required]
        public SnowFlakeId GroupId { get; set; }

        [Required]
        [Range(0.01, 9999999999.99, ErrorMessage = "Amount must be between 0.01 and 9999999999.99")]
        public decimal Amount { get; set; }

        [Required]
        public short TransactionTypeId { get; set; }

        public long? CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Iso8601Date]
        public required string TransactionDate { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one split entry is required.")]
        public required List<ExpenseSplitDto> Split { get; set; }

        [Required]
        public required PaymentDto Payment { get; set; }
    }
}
