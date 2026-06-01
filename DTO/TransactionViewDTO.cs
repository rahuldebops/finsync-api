namespace finsyncapi.DTO
{
    public class TransactionViewDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int? AccountId { get; set; }
        public int? CategoryId { get; set; }
        public decimal Amount { get; set; }
        public short Type { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string TransactionDate { get; set; }
        public bool? IsShared { get; set; }
        public Guid? SharedBy { get; set; }
        public short? SharedStatus { get; set; }
    }
}
