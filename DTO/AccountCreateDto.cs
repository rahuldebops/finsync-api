using System;
using static finsyncapi.Helpers.ValidationAttributes;

namespace finsyncapi.Dto
{
    public class AccountCreateDto
    {
        public string Name { get; set; } = null!;
        public short AccountTypeId { get; set; }
        public decimal Balance { get; set; }
        [Iso8601Date]
        public string? BalanceAsOf { get; set; }
        public string CurrencyCode { get; set; } = null!;
    }
}
