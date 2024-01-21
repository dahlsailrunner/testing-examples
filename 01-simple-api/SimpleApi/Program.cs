using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Serilog.Formatting.Compact;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(new RenderedCompactJsonFormatter())
    );

    builder.Services.AddProblemDetails(opts =>
        opts.CustomizeProblemDetails = Helpers.CustomizeResponse);

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    app.MapGet("/weatherforecast",
        (IWebHostEnvironment env, string? postalCode) =>
        {
            if (string.IsNullOrEmpty(postalCode))
                throw new ValidationException("Postal Code is required.");

            if (string.Equals(postalCode, "error", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("Simulated error!");

            if (postalCode.Length != 5)
                throw new ValidationException("Postal Code should be 5 digits long.");

            if (!postalCode.All(char.IsDigit))
                throw new ValidationException("Postal Code should be all digits.");

            var forecast = Helpers.GetForecast();
            return new ForecastAndEnv(env.EnvironmentName, forecast);
        });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { } // needed for integration tests

public record ForecastAndEnv(string Environment, WeatherForecast[] Forecast);
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal static class Helpers
{
    internal static WeatherForecast[] GetForecast()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        return Enumerable.Range(1, 5)
            .Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
    }

    internal static void CustomizeResponse(ProblemDetailsContext pdc)
    {
        var ex = pdc.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        switch (ex)
        {
            case BadHttpRequestException badReqEx:
                pdc.ProblemDetails.Detail = badReqEx.Message;
                pdc.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                break;
            case ValidationException valEx:
                pdc.ProblemDetails.Detail = valEx.Message;
                pdc.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                break;
            default:
                pdc.ProblemDetails.Detail = pdc.ProblemDetails.Detail;
                break;
        }

        pdc.ProblemDetails.Status = pdc.HttpContext.Response.StatusCode;
    }
}