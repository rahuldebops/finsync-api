using System.ComponentModel.DataAnnotations;
using finsyncapi.Models;

namespace finsyncapi.Dto
{
    public class GroupJoinDto
    {
        [Required]
        public SnowFlakeId? GroupId { get; set; }
    }
}
