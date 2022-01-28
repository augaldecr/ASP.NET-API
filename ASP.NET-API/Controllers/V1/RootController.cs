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

        public RootController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }


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

            // Accounts actions
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("Login", new { }), description: "self", method: "POST"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("Register", new { }), description: "self", method: "POST"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("Refresh", new { }), description: "self", method: "GET"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("ConvertToAdmin", new { }), description: "self", method: "POST"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("ConvertToNoAdmin", new { }), description: "self", method: "POST"));

            // Book Actions
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetBooks", new { }), description: "self", method: "GET"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetBookById", new { }), description: "self", method: "GET"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("UpdateBook", new { }), description: "self", method: "PUT"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("PatchBook", new { }), description: "self", method: "PATCH"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("DeleteBook", new { }), description: "self", method: "DELETE"));

            //Comments Actions

            return dataHateoas;
        }
    }
}
