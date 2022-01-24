﻿using Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class AuthorCreateDTO
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "The field {0} must not exceed {1} characters")]
        [CapitalFirstLetterAttribute]
        public string Name { get; set; }
    }
}
