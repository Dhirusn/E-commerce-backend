using ProductService.Shared;

namespace ProductService.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.StatusCode = ex switch
                {
                    ArgumentNullException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.ContentType = "application/json";

                var result = Result<string>.Fail(GetErrorCode(ex), ex.Message);

                await context.Response.WriteAsJsonAsync(result);
            }
        }

        private string GetErrorCode(Exception ex) => ex switch
        {
            ArgumentNullException => ErrorCodes.Validation,
            UnauthorizedAccessException => ErrorCodes.Unauthorized,
            KeyNotFoundException => ErrorCodes.NotFound,
            _ => ErrorCodes.Unknown
        };
    }
}
