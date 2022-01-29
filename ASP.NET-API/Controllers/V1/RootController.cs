using ASP.NET_API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace ASP.NET_API.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public RootController(IAuthorizationService authorizationService) => _authorizationService = authorizationService;


        [HttpGet(Name = "GetRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> Get()
        {
            var isAdmin = await _authorizationService.AuthorizeAsync(User, PoliciesHelper.IsAnAdmin);

            var dataHateoas = new List<DataHATEOAS>();

            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetRoot", new { }), description: "self", method: "GET"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetAuthors", new { }), description: "authors", method: "GET"));
            
            if (isAdmin.Succeeded)
            {
                dataHateoas.Add(new DataHATEOAS(link: Url.Link("CreateAuthor", new { }), description: "create-author", method: "POST"));
                dataHateoas.Add(new DataHATEOAS(link: Url.Link("CreateBook", new { }), description: "create-book", method: "POST"));
            }

            return dataHateoas;
        }
    }
}
