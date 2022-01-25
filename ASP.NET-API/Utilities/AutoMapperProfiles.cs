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
            CreateMap<BookCreateDTO, Book>();
            CreateMap<Book, BookDTO>();
            CreateMap<CommentCreateDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }
    }
}
