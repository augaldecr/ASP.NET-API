using ASP.NET_API.Data;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Entities;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsControllers : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public AuthorsControllers(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuthorDTO>>> Get()
        {
            var authors = await _context.Authors.ToListAsync();
            return Ok(mapper.Map<List<AuthorDTO>>(authors));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorDTO>> Get(int id)
        {
            var autor = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
                return NotFound();

            return mapper.Map<AuthorDTO>(autor);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<AuthorDTO[]>> Get([FromRoute] string name)
        {
            var authors = await _context.Authors.Where(x => x.Name.Contains(name)).ToArrayAsync();

            if (authors is null)
                return NotFound();

            return mapper.Map<AuthorDTO[]>(authors);
        }

        [HttpPost]
        public async Task<ActionResult> Post(AuthorCreateDTO authorCreateDTO)
        {
            var author = mapper.Map<Author>(authorCreateDTO);

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
