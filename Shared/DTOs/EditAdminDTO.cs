using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class EditAdminDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
