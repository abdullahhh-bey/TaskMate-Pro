using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Net;
using TaskMate.Core.DTO.ErrorDTO;

namespace TaskMate.Application.Services.CustomMiddlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError("Error Occured for {Path} at {Time}:", context.Request?.Path, DateTime.UtcNow);

            context.Response.ContentType = "application/json";

            var message = "Unhandled Exception";
            var statusCode = (int)HttpStatusCode.InternalServerError;

            if(exception is KeyNotFoundException)
            {
                message = "Not Found";
                statusCode = (int)HttpStatusCode.NotFound;
            }

            else if(exception is BadHttpRequestException)
            {
                message = "Invalid Information";
                statusCode = (int)HttpStatusCode.BadRequest;
            }

            else if(exception is InvalidOperationException)
            {
                message = "Invalid Request";
                statusCode = (int)HttpStatusCode.NotImplemented;
            }

            else if(exception is UnauthorizedAccessException)
            {
                message = "Unauthorized Attempt";
                statusCode = (int)HttpStatusCode.Unauthorized;
            }

            else if (exception is SqlException)
            {
                message = "Failed to load from Database";
                statusCode = (int)HttpStatusCode.InternalServerError;
            }

            var errorResponse = new ErrorMessage
            {
                Message = message,
                StatusCode = statusCode.ToString(),
                Details = exception.Message
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(errorResponse);
            return true;
        }
    }
}
