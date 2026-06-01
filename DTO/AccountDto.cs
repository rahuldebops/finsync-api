using finsyncapi.Models;

namespace finsyncapi.Dto
{
    public class AccountDto
    {
        public SnowFlakeId Id { get; set; }
        public string Name { get; set; } = null!;
        public string AccountTypeName { get; set; } = null!;
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; } = null!;
    }
}
