using System.Net;
using Microsoft.AspNetCore.Http;
using Theatrical.Dto.ResponseWrapperFolder;

namespace Theatrical.Services.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            int statusCode;
            ErrorCode errorCode;
            
            if (ex is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = ErrorCode.BadRequest;
            }
            else if (ex is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status401Unauthorized;
                errorCode = ErrorCode.Unauthorized;
            }
            else if (ex is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                errorCode = ErrorCode.NotFound;
            }
            else
            {
                // For any other exception, set a generic 500 Internal Server Error status code.
                statusCode = StatusCodes.Status500InternalServerError;
                errorCode = ErrorCode.ServerError;
            }
            
            var response = new ApiResponse(errorCode, $"{ex.Message}");
            
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(response.ToString());
        }
    }
}