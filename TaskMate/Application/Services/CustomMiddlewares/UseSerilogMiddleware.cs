using static System.Net.WebRequestMethods;

namespace TaskMate.Application.Services.CustomMiddlewares
{
    public class UseSerilogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UseSerilogMiddleware> _logger;

        public UseSerilogMiddleware(RequestDelegate next, ILogger<UseSerilogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task RequestLogger(HttpContext context)
        {
            _logger.LogInformation("Incoming Request: {Method} {Path} at {Time}", 
            context.Request?.Method, context.Request?.Path.Value, DateTime.UtcNow);
        
            await _next(context);

            _logger.LogInformation("Outgoing Response: {StatusCode} at {Time}",
            context.Response?.StatusCode, DateTime.UtcNow);
        }

    }
}
