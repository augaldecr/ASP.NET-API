using ASP.NET_API.Data;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Entities;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CommentsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
        {
            var bookExist = await _context.Books.AnyAsync(b => b.Id == bookId);

            if (!bookExist)
                return NotFound();

            var comments = await _context.Comments.Where(c => c.BookId == bookId).ToListAsync();

            return _mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int bookId, CommentCreateDTO commentCreateDTO)
        {
            var bookExist = await _context.Books.AnyAsync(b => b.Id == bookId);

            if (!bookExist)
                return NotFound();

            var comment = _mapper.Map<Comment>(commentCreateDTO);
            comment.BookId = bookId;

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
