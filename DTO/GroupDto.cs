using finsyncapi.Models;

namespace finsyncapi.Dto
{
    public class GroupDto
    {
        public SnowFlakeId Id { get; set; }
        public string GroupName { get; set; } = null!;
        public short GroupRole { get; set; }
    }
}
