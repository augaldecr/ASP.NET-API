using Shared.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class BookPatchDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "The field {0} must not exceed {1} characters")]
        [CapitalFirstLetterAttribute]
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
