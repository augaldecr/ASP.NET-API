using ASP.NET_API.Data;
using ASP.NET_API.Helpers;
using ASP.NET_API.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Entities;

namespace ASP.NET_API.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PoliciesHelper.IsAnAdmin)]
    [Route("api/authors")]
    //[Route("api/v1/authors")]
    [HeaderAttribute("x-version", "1")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AuthorsControllers : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;

        public AuthorsControllers(ApplicationDbContext context,
                                  IMapper mapper,
                                  IConfiguration configuration,
                                  IAuthorizationService authorizationService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetAuthors")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<List<AuthorDTO>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.Authors.AsQueryable();
            await HttpContext.InsertPaginationParametersInHeaders(queryable);
            var authors = await queryable.OrderBy(a => a.Name).Paginate(paginationDTO).ToListAsync();
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        //[HttpGet("TestConfiguration")]
        //public async Task<ActionResult<string>> TestConfiguration()
        //{
        //    //return _configuration["connectionStrings:defaultConnection"];
        //    //return _configuration["ASPNETCORE_ENVIRONMENT"];
        //    return _configuration["Test"];
        //}

        [HttpGet("{id:int}", Name = "GetAuthorById")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200)]
        public async Task<ActionResult<AuthorDTO>> Get(int id)
        {
            var autor = await _context.Authors.Include(a => a.AuthorsBooks)
                                                    .ThenInclude(x => x.Book)
                                              .FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
                return NotFound();

            var dto = _mapper.Map<AuthorDTOWithBooks>(autor);
            return dto;
        }

        [HttpGet("{name}", Name = "GetAuthorByName")]
        public async Task<ActionResult<AuthorDTO[]>> GetAuthorByName([FromRoute] string name)
        {
            var authors = await _context.Authors.Where(x => x.Name.Contains(name)).ToArrayAsync();

            if (authors is null)
                return NotFound();

            return _mapper.Map<AuthorDTO[]>(authors);
        }

        [HttpPost(Name = "CreateAuthor")]
        public async Task<ActionResult> Post(AuthorCreateDTO authorCreateDTO)
        {
            var author = _mapper.Map<Author>(authorCreateDTO);

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            var authorDTO = _mapper.Map<AuthorDTO>(author);

            return CreatedAtRoute("GetAuthorById", new { id = author.Id }, authorDTO);
        }

        // api/authors/3
        [HttpPut("{id:int}", Name = "UpdateAuthor")]
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

        /// <summary>
        /// Deletes an Author
        /// </summary>
        /// <param name="id">Id of the Author to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteAuthor")]
        public async Task<ActionResult> Delete(int id)
        {
            if (await AuthorExist(id))
                return NotFound();

            _context.Authors.Remove(new Author { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> AuthorExist(int id) => await _context.Authors.AnyAsync(b => b.Id == id);
    }
}
