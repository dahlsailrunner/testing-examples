using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace CarvedRock.Catalog.Api;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, 
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ApplicationException:
                logger.LogInformation(exception, "Bad Request has occurred.");
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                break;
            case KeyNotFoundException:
                // nothing to log here (request logging handles)
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                break;
            default:
                logger.LogError(exception, "An unhandled exception has occurred.");
                httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                break;
        }
        
        return ValueTask.FromResult(false);
    }
    
    public static void CustomizeResponse(ProblemDetailsContext pdc)
    {
        var ex = pdc.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        switch (ex)
        {
            case ApplicationException appEx:
                pdc.ProblemDetails.Detail = appEx.Message;
                break;
            case KeyNotFoundException notFound:
                pdc.ProblemDetails.Detail = notFound.Message;
                pdc.ProblemDetails.Title = "Resource not found";
                break;
            default:
                pdc.ProblemDetails.Detail = pdc.ProblemDetails.Detail;
                break;
        }

        pdc.ProblemDetails.Status = pdc.HttpContext.Response.StatusCode;
    }
}