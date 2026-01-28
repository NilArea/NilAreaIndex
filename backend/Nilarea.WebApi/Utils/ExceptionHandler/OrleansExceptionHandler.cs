using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NilArea.Api.Utils.ExceptionHandler;

public class OrleansExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not OrleansException)
            return false;

        logger.LogError(exception, "OrleansException Occurred");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = exception.Message,
                Status = StatusCodes.Status500InternalServerError
            }
        };

        return await problemDetailsService.TryWriteAsync(context);
    }
}
