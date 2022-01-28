using ASP.NET_API.Data;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Entities;

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
        [HttpGet(Name = "GetBooks")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            var books = await _context.Books.ToListAsync();

            var booksDTO = _mapper.Map<List<BookDTO>>(books);

            return Ok(booksDTO);
        }

        //GET: api/Books/5
        [HttpGet("{id:int}", Name = "GetBookById")]
        public async Task<ActionResult<BookDTOWithAuthors>> GetBook(int id)
        {
            var book = await _context.Books.Include(b => b.Comments)
                                           .Include(b => b.AuthorsBooks)
                                                .ThenInclude(a => a.Author)
                                           .FirstOrDefaultAsync(b => b.Id == id);

            if (book is null)
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
        [HttpPut("{id}", Name = "UpdateBook")]
        public async Task<IActionResult> PutBook(int id, BookCreateDTO bookCreateDTO)
        {
            var bookDB = await _context.Books.Include(b => b.AuthorsBooks)
                                             .FirstOrDefaultAsync(b => b.Id == id);

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
        [HttpPost(Name = "CreateBook")]
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

            return CreatedAtRoute("GetBookById", new { id = book.Id }, book);
        }


        // Must install this nuget to use Patch
        // Microsoft.AspNetCore.Mvc.NewtonsoftJson
        [HttpPatch("{id:int}", Name = "PatchBook")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> patchDocument)
        {
            if (patchDocument is null)
            {
                return BadRequest();
            }

            var bookDB = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);

            if (bookDB is null)
            {
                return NotFound();
            }

            var bookDTO = _mapper.Map<BookPatchDTO>(bookDB);

            patchDocument.ApplyTo(bookDTO, ModelState);

            var isValid = TryValidateModel(bookDTO);

            if (!isValid)
                return BadRequest();

            _mapper.Map(bookDTO, bookDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Books/5
        [HttpDelete("{id:int}", Name = "DeleteBook")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (await BookExist(id))
                return NotFound();

            _context.Books.Remove(new Book { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> BookExist(int id) => await _context.Books.AnyAsync(b => b.Id == id);

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
