using Microsoft.EntityFrameworkCore;

namespace ASP.NET_API.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParametersInHeaders<T>(this HttpContext httpContext,
                                                                        IQueryable<T> queryable)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double qty = await queryable.CountAsync();
            httpContext.Response.Headers.Add("recordsQty", qty.ToString());
        }
    }
}
