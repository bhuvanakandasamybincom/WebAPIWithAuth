namespace WebAPI.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandleMiddleware> _logger;

        public ExceptionHandleMiddleware(RequestDelegate next,
            ILogger<ExceptionHandleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        /// <summary>
        /// Convention-based middleware to handle log
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {

            try
            {
                _logger.LogInformation("Middleware handling request, {path}",httpContext.Request.Path);
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                //Log exception error message
                var ErrMessage = $"{ex.GetType().ToString()}:{ex.Message}";
                APIResult<string> objResponse = new();

                if (ex.InnerException is not null)
                {
                    //Log the inner exception error message
                    ErrMessage += $"{ex.InnerException.GetType().ToString()}:{ex.InnerException.Message}";

                }
                _logger.LogError(ErrMessage);
                objResponse.ResponseCode = 500;
                objResponse.Message = ErrMessage;
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsJsonAsync(objResponse);
            }


        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandleMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandleMiddleware>();
        }
    }
}
