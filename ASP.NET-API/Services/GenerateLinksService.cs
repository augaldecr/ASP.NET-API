using ASP.NET_API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Shared.DTOs;

namespace ASP.NET_API.Services
{
    public class GenerateLinksService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public GenerateLinksService(IAuthorizationService authorizationService,
                                    IHttpContextAccessor httpContextAccessor,
                                    IActionContextAccessor actionContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        private async Task<bool> IsAdmin()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var result = await _authorizationService.AuthorizeAsync(httpContext.User, PoliciesHelper.IsAnAdmin);
            return result.Succeeded;
        }

        private IUrlHelper BuildUrlHelper()
        {
            var factory = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        public async Task GenerateLinks(AuthorDTO authorDTO)
        {
            var isAdmin = await IsAdmin();
            var Url = BuildUrlHelper();

            // Author actions
            authorDTO.Links.Add(new DataHATEOAS(link: Url.Link("GetAuthorById", new { }), description: "self", method: "GET"));
            authorDTO.Links.Add(new DataHATEOAS(link: Url.Link("GetAuthorByName", new { }), description: "author-by-name", method: "GET"));

            if (isAdmin)
            {
                authorDTO.Links.Add(new DataHATEOAS(link: Url.Link("UpdateAuthor", new { }), description: "author-update", method: "PUT"));
                authorDTO.Links.Add(new DataHATEOAS(link: Url.Link("DeleteAuthor", new { }), description: "author-delete", method: "DELETE"));
            }
        }
    }
}
