using System;

namespace finsyncapi.Dto
{
    public class AccountCreateDto
    {
        public string Name { get; set; } = null!;
        public short AccountTypeId { get; set; }
        public decimal Balance { get; set; }
        public DateTime BalanceAsOf { get; set; }
        public string CurrencyCode { get; set; } = null!;
    }
}
