using AutoMapper;
using Shared.DTOs;
using Shared.Entities;

namespace ASP.NET_API.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AuthorCreateDTO, Author>();
            CreateMap<Author, AuthorDTO>();
            CreateMap<Author, AuthorDTOWithBooks>()
                .ForMember(authorDTO => authorDTO.Books, opt => opt.MapFrom(MapAuthorDTOBooks));
            CreateMap<BookCreateDTO, Book>()
                .ForMember(book => book.AuthorsBooks, opt => opt.MapFrom(MapAuthorsBooks));
            CreateMap<Book, BookDTO>();
            CreateMap<Book, BookDTOWithAuthors>()
                .ForMember(b => b.Authors, opt => opt.MapFrom(MapBookDTOAuthors));
            CreateMap<CommentCreateDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }

        private List<BookDTO> MapAuthorDTOBooks(Author autor, AuthorDTO authorDTO)
        {
            var result = new List<BookDTO>();

            if (autor.AuthorsBooks is null) { return result; }

            foreach (var authorBook in autor.AuthorsBooks)
            {
                result.Add(new BookDTO() { Id = authorBook.BookId, Title = authorBook.Book.Title });
            }

            return result;
        }

        private List<AuthorDTO> MapBookDTOAuthors(Book book, BookDTO bookDTO)
        {
            var result = new List<AuthorDTO>();

            if (book.AuthorsBooks is null) { return result; }

            foreach (var authorBook in book.AuthorsBooks)
            {
                result.Add(new AuthorDTO() { Id = authorBook.AuthorId, Name = authorBook.Author.Name });
            }

            //result.AddRange(bookCreateDTO.AuthorsId.Select( a => new AuthorBook { AuthorId = a }));

            return result;
        }

        private List<AuthorBook> MapAuthorsBooks(BookCreateDTO bookCreateDTO, Book book)
        {
            var result = new List<AuthorBook>();

            if (bookCreateDTO.AuthorsId == null) { return result; }

            foreach (var authorId in bookCreateDTO.AuthorsId)
            {
                result.Add(new AuthorBook() { AuthorId = authorId });
            }

            //result.AddRange(bookCreateDTO.AuthorsId.Select( a => new AuthorBook { AuthorId = a }));

            return result;
        }
    }
}
