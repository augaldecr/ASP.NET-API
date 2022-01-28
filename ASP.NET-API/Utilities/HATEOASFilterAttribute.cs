using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP.NET_API.Utilities
{
    public class HATEOASFilterAttribute : ResultFilterAttribute
    {
        protected bool mustIncludeHATEOAS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;

            if (!isSuccededResponse(result))
            {
                return false;
            }

            var header = context.HttpContext.Request.Headers["includeHATEOAS"];

            if (header.Count == 0)
            {
                return false;
            }

            var value = header[0];

            if (!value.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private bool isSuccededResponse(ObjectResult result)
        {
            if (result is null || result.Value is null)
            {
                return false;
            }

            if (result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }

            return true;
        }
    }
}
