namespace Shared.DTOs
{
    public class BookDTO 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? PublicationDate { get; set; }
        public List<CommentDTO> Comments { get; set; }
    }
}
