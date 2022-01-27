using ASP.NET_API.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Entities;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/authors")]
    public class AuthorsControllers : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthorsControllers(ApplicationDbContext context, 
                                  IMapper mapper, 
                                  Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<AuthorDTOWithBooks>>> Get()
        {
            var authors = await _context.Authors.ToListAsync();
            return Ok(_mapper.Map<List<AuthorDTO>>(authors));
        }

        //[HttpGet("TestConfiguration")]
        //public async Task<ActionResult<string>> TestConfiguration()
        //{
        //    //return _configuration["connectionStrings:defaultConnection"];
        //    //return _configuration["ASPNETCORE_ENVIRONMENT"];
        //    return _configuration["Test"];
        //}

        [HttpGet("{id:int}", Name = "getAuthorById")]
        public async Task<ActionResult<AuthorDTO>> Get(int id)
        {
            var autor = await _context.Authors.Include(a => a.AuthorsBooks)
                                                    .ThenInclude(x => x.Book)        
                                              .FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
                return NotFound();

            return _mapper.Map<AuthorDTOWithBooks>(autor);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<AuthorDTO[]>> Get([FromRoute] string name)
        {
            var authors = await _context.Authors.Where(x => x.Name.Contains(name)).ToArrayAsync();

            if (authors is null)
                return NotFound();

            return _mapper.Map<AuthorDTO[]>(authors);
        }

        [HttpPost]
        public async Task<ActionResult> Post(AuthorCreateDTO authorCreateDTO)
        {
            var author = _mapper.Map<Author>(authorCreateDTO);

            await _context.Authors.AddAsync(author);   
            await _context.SaveChangesAsync();

            var authorDTO = _mapper.Map<AuthorDTO>(author);

            return CreatedAtRoute("getAuthorById", new { id = author.Id }, authorDTO);
        }

        // api/authors/3
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, AuthorCreateDTO authorCreateDTO)
        {
            var exist = await _context.Authors.AnyAsync(a => a.Id == id);

            if (!exist)
            {
                return NotFound();
            }

            var author = _mapper.Map<Author>(authorCreateDTO);
            author.Id = id;

            _context.Update(author);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // api/authors/3
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if(await AuthorExist(id))
                return NotFound();

            _context.Authors.Remove(new Author { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> AuthorExist(int id) => await _context.Authors.AnyAsync(b => b.Id == id);
    }
}
