namespace ASP.NET_API.Middlewares
{
    public static class LogHttpResponsesMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogHttpResponses(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogHttpResponsesMiddleware>();
        }
    }

    public class LogHttpResponsesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogHttpResponsesMiddleware> _logger;

        public LogHttpResponsesMiddleware(RequestDelegate next, ILogger<LogHttpResponsesMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        //Invoke or InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            using MemoryStream ms = new();
            var originalBody= context.Request.Body;
            context.Response.Body = ms; 

            await _next(context);

            ms.Seek(0, SeekOrigin.Begin);
            string response = new StreamReader(ms).ReadToEnd();
            ms.Seek(0, SeekOrigin.Begin);

            await ms.CopyToAsync(originalBody);
            context.Response.Body = originalBody;

            _logger.LogInformation(response);   
        }
    }
}
