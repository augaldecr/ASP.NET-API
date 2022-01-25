using Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class CommentCreateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 1200, ErrorMessage = "The field {0} must not exceed {1} characters")]
        [CapitalFirstLetterAttribute]
        public string Content { get; set; }
    }
}
