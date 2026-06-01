using finsyncapi.Models;

namespace finsyncapi.Dto
{
    public class MasterDto
    {
        public object Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class CategoryDto
    {
        public SnowFlakeId Id { get; set; }
        public string CategoryName { get; set; }
        public short TransactionTypeId { get; set; }
    }
}
