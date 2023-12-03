using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace CarvedRock.Catalog.Api;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ApplicationException)
            logger.LogInformation(exception, "Bad Request has occurred.");
        else
            logger.LogError(exception, "An unhandled exception has occurred.");
        
        return ValueTask.FromResult(false);
    }
    
    public static void CustomizeResponse(ProblemDetailsContext pdc)
    {
        var ex = pdc.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (ex is not ApplicationException appEx) return;
    
        pdc.ProblemDetails.Detail = appEx.Message;
        pdc.ProblemDetails.Status = (int) HttpStatusCode.BadRequest;
        pdc.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
    }
}