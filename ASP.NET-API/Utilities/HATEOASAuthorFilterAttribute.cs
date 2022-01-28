using ASP.NET_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.DTOs;

namespace ASP.NET_API.Utilities
{
    public class HATEOASAuthorFilterAttribute : HATEOASFilterAttribute
    {
        private readonly GenerateLinksService _generateLinks;

        public HATEOASAuthorFilterAttribute(GenerateLinksService generateLinks)
        {
            _generateLinks = generateLinks;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var mustInclude = mustIncludeHATEOAS(context);

            if (!mustInclude)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;

            var authorDTO = result.Value as AuthorDTO;

            if (authorDTO is null)
            {
                var authorsDTO = result.Value as List<AuthorDTO> ??
                                throw new ArgumentNullException("An instance of AuthorDTO was expected");

                authorsDTO.ForEach(async author => await _generateLinks.GenerateLinks(author));
                result.Value = authorsDTO;
            }
            else
            {
                await _generateLinks.GenerateLinks(authorDTO);
            }

            await next();
        }
    }
}
