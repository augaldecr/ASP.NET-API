using Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace Shared.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 120, ErrorMessage = "The field {0} must not exceed {1} characters")]
        [CapitalFirstLetterAttribute]
        public string Title { get; set; }

        public IEnumerable<Comment> Comments { get; set; }
        public List<AuthorBook> AuthorsBooks { get; set; }
    }
}
