using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NilArea.Interfaces.Exceptions;

namespace NilArea.Web.Utils.ExceptionHandler;

public class OrleansExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not OrleansException oe)
            return false;

        logger.LogError(exception, "OrleansException Occurred");

        var context = oe switch
        {
            AccountException ae => HandleAccountException(httpContext, ae),
            AuthenticationException aue => HandleAuthenticationException(httpContext, aue),
            _ => HandleDefaultException(httpContext, oe)
        };

        return await problemDetailsService.TryWriteAsync(context);
    }

    /// <summary>
    ///     处理其他 Orleans异常
    /// </summary>
    private static ProblemDetailsContext HandleDefaultException(
        HttpContext httpContext,
        OrleansException exception)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "An server error occurred",
                Status = StatusCodes.Status500InternalServerError
            }
        };
        return context;
    }

    /// <summary>
    ///     处理帐号管理异常
    /// </summary>
    private static ProblemDetailsContext HandleAccountException(
        HttpContext httpContext,
        AccountException exception)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = exception.Message,
                Status = StatusCodes.Status400BadRequest
            }
        };
        return context;
    }

    /// <summary>
    ///     处理身份验证异常
    /// </summary>
    private static ProblemDetailsContext HandleAuthenticationException(
        HttpContext httpContext,
        AuthenticationException exception)
    {
        var code = exception.Result switch
        {
            AuthenticationResult.Failed => StatusCodes.Status400BadRequest,
            AuthenticationResult.Unauthorized => StatusCodes.Status401Unauthorized,
            AuthenticationResult.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
        httpContext.Response.StatusCode = code;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = exception.Message,
                Status = code
            }
        };
        return context;
    }
}
