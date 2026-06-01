using System;
using finsyncapi.Models;

namespace finsyncapi.Dto
{
    public class ProfileDto
    {
        public SnowFlakeId ProfileId { get; set; }
        public SnowFlakeId UserId { get; set; }
        public string ProfileName { get; set; }
        public string? Description { get; set; }
    }
}
