using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP.NET_API.Data;
using Shared.Entities;
using Shared.DTOs;
using AutoMapper;

namespace ASP.NET_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //GET: api/Books
       [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _context.Books.ToListAsync();

            var booksDTO = _mapper.Map<List<BookDTO>>(books);

            return Ok(booksDTO);
        }

        //GET: api/Books/5
        [HttpGet("{id:int}", Name = "GetBook")]
        public async Task<ActionResult<BookDTOWithAuthors>> GetBook(int id)
        {
            var book = await _context.Books.Include(b => b.Comments)
                                           .Include(b => b.AuthorsBooks)
                                                .ThenInclude(a => a.Author)
                                           .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            if (book.AuthorsBooks is not null)
            {
                book.AuthorsBooks = book.AuthorsBooks.OrderBy(a => a.Order).ToList();
            }

            return _mapper.Map<BookDTOWithAuthors>(book);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookCreateDTO bookCreateDTO)
        {
            var bookDB = await _context.Books.Include(b => b.AuthorsBooks)
                                             .FirstOrDefaultAsync(b => b.Id==id);

            if (bookDB is null)
            {
                return NotFound();
            }

            bookDB = _mapper.Map(bookCreateDTO, bookDB);

            OrderAuthors(bookDB);

            await _context.SaveChangesAsync();            
            return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(BookCreateDTO bookCreateDTO)
        {
            if (bookCreateDTO.AuthorsId is null)
            {
                return BadRequest("Can't add a book with no authors");
            }

            var authorsIds = await _context.Authors.Where(a => bookCreateDTO.AuthorsId.Contains(a.Id))
                                                .Select(a => a.Id)
                                                .ToListAsync();

            if (bookCreateDTO.AuthorsId.Count != authorsIds.Count)
            {
                return BadRequest("One or more of the selected authors does not exist in the database");
            }

            var book = _mapper.Map<Book>(bookCreateDTO);

            OrderAuthors(book);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var bookDTO = _mapper.Map<BookDTO>(book);

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void OrderAuthors(Book book)
        {
            if (book.AuthorsBooks is not null)
            {
                for (int i = 0; i < book.AuthorsBooks.Count; i++)
                {
                    book.AuthorsBooks[i].Order = i;
                }
            }
        }
    }
}
