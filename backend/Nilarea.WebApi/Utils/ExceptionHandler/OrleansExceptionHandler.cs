using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NilArea.Contracts.Exceptions;

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
            AccountException ace => HandleAccountException(httpContext, ace),
            AuthenticationException aue => HandleAuthenticationException(httpContext, aue),
            ConfirmException cce => HandleConfirmException(httpContext, cce),
            _ => HandleDefaultException(httpContext, oe)
        };

        return await problemDetailsService.TryWriteAsync(context);
    }

    private ProblemDetailsContext HandleConfirmException(HttpContext httpContext, ConfirmException exception)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "Invalid Confirm Code",
                Status = StatusCodes.Status400BadRequest,
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            }
        };
        return context;
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
                Title = "An Error Occured",
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
                Title = "Invalid Account Request",
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
        httpContext.Response.StatusCode = (int)exception.Result;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "Failed to Authenticate",
                Status = (int)exception.Result
            }
        };
        return context;
    }
}