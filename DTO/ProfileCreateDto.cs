using System.ComponentModel.DataAnnotations;

namespace finsyncapi.Dto
{
    public class ProfileCreateDto
    {
        [Required]
        [StringLength(10)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "ProfileName must be alphanumeric only.")]
        public string ProfileName { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
    }
}
