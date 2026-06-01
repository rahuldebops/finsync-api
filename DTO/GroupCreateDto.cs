using System.ComponentModel.DataAnnotations;

namespace finsyncapi.Dto
{
    public class GroupCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Group name must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Group name must be alphanumeric and can contain spaces only.")]
        public string GroupName { get; set; } = null!;
    }
}