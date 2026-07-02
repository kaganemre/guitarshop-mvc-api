using System.Net;
using GuitarShopApp.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GuitarShopApp.WebAPI.Attributes;

public class NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        
        if (exception is not NotFoundException)
            return false;
        
        logger.LogError(exception, "An unexpected error occurred");
        
        var model = new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = "An unexpected error occurred",
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        httpContext.Response.StatusCode = (int)model.Status;

        await httpContext.Response
            .WriteAsJsonAsync(model, cancellationToken);

        return true;
    }
}