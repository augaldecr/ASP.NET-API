using ASP.NET_API.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Entities;
using System.Security.Claims;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/books/{bookId:int}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentsController(ApplicationDbContext context, 
                                  IMapper mapper,
                                  UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet(Name = "GetCommentsByBookId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[AllowAnonymous]
        public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
        {
            var bookExist = await _context.Books.AnyAsync(b => b.Id == bookId);

            if (!bookExist)
                return NotFound();

            var comments = await _context.Comments.Where(c => c.BookId == bookId).ToListAsync();

            return _mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpGet("{id:int}", Name = "GetCommentById")]
        public async Task<ActionResult<CommentDTO>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment is null)
            {
                return NotFound();
            }

            return _mapper.Map<CommentDTO>(comment);
        }

        [HttpPut("{id:int}", Name = "UpdateComment")]
        public async Task<ActionResult> Put(int id, int bookId, CommentCreateDTO commentCreateDTO)
        {
            var bookExist = await _context.Books.AnyAsync(b => b.Id == bookId);

            if (!bookExist)
                return NotFound();

            var commentExist = await _context.Comments.AnyAsync(c => c.Id == id);

            if (commentExist)
            {
                return NotFound();
            }

            var comment = _mapper.Map<Comment>(commentCreateDTO);
            comment.Id = id;
            comment.BookId = bookId;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost(Name = "CreateComment")]
        public async Task<ActionResult> Post(int bookId, CommentCreateDTO commentCreateDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
            var email = emailClaim?.Value;
            var user = await _userManager.FindByEmailAsync(email);
            var userId = user.Id;
            var bookExist = await _context.Books.AnyAsync(b => b.Id == bookId);

            if (!bookExist)
                return NotFound();

            var comment = _mapper.Map<Comment>(commentCreateDTO);
            comment.BookId = bookId;
            comment.UserId = userId;    

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            var commentDTO = _mapper.Map<CommentDTO>(comment);

            return CreatedAtRoute("GetCommentById", new { id = comment.Id, bookId = bookId }, commentDTO);
        }
    }
}
