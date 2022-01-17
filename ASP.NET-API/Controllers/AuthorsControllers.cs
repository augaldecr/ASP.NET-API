using ASP.NET_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsControllers : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorsControllers(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Author>>> Get()
        {
            return await _context.Authors.Include(a => a.Books).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Author author)
        {
            await _context.Authors.AddAsync(author);   
            await _context.SaveChangesAsync();
            return Ok();
        }

        // api/authors/3
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Author author)
        {
            if (author.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            var exist = await _context.Authors.AnyAsync(a => a.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            _context.Update(author);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // api/authors/3
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            //var author = await _context.Authors.FindAsync(id);

            //if (author is null)
            //{
            //    return NotFound();
            //}

            var exist = await _context.Authors.AnyAsync(a => a.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            _context.Authors.Remove(new Author { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
