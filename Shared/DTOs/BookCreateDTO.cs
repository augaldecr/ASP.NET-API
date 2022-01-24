using Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class BookCreateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "The field {0} must not exceed {1} characters")]
        [CapitalFirstLetterAttribute]
        public string Title { get; set; }
    }
}
