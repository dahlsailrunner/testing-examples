using System.IdentityModel.Tokens.Jwt;
using Asp.Versioning.ApiExplorer;
using CarvedRock.Catalog.Api;
using CarvedRock.Catalog.Api.LogEnrichers;
using CarvedRock.Catalog.Api.StartupServices;
using CarvedRock.Catalog.Api.Swagger;
using CarvedRock.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
   
    builder.Services.AddControllers();
    builder.Services.AddHealthChecks();
    builder.Services
        .AddMvcCore(options => { options.AddBaseAuthorizationFilters(configuration); })
        .AddApiExplorer();

    builder.Services.AddProblemDetails(opts => { opts.CustomizeProblemDetails = CustomExceptionHandler.CustomizeResponse; });
    builder.Services.AddExceptionHandler<CustomExceptionHandler>();

    builder.Services.AddLogic()// defined in StartupServices folder
        .AddCustomApiVersioning()
        .AddSwaggerFeatures()
        .AddTransient<ILogEventEnricher, StandardEnricher>()
        .AddHttpContextAccessor();

    var connStr = configuration.GetConnectionString("DbContext");

    builder.Services.AddDbContext<LocalContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DbContext")));

    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
    builder.Services
        .AddAuthentication("Bearer")
        .AddJwtBearer(options =>
        {
            options.Authority = configuration.GetValue<string>("Authentication:Authority");
            options.Audience = configuration.GetValue<string>("Authentication:ApiName");
        });

    builder.Host.UseSerilog(((context, services, loggerConfig) =>
    {
        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "CarvedRock_Catalog.Api") // or entry assembly name
            .WriteTo.Console()
            //.WriteTo.Seq("http://host.docker.internal:5341");  // comment this IN if using docker
            .WriteTo.Seq("http://localhost:5341");       // comment this OUT if NOT using docker
    }));

    var app = builder.Build();

    if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
    {
        var forwardedHeaderOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };
        forwardedHeaderOptions.KnownNetworks.Clear();
        forwardedHeaderOptions.KnownProxies.Clear();
        app.UseForwardedHeaders(forwardedHeaderOptions);
    }

    var corsOrigins = configuration.GetValue<string>("CORSOrigins")?.Split(",");
    if (corsOrigins!= null && corsOrigins.Length != 0)
    {
        app.UseCors(bld => bld
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
    }
   
    var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.Services.InitializeDatabase(app.Environment.EnvironmentName);

    app
        .UseSwaggerFeatures(configuration, apiVersionProvider, app.Environment)
        .UseAuthentication()
        .UseCustomRequestLogging()
        .UseExceptionHandler()
        .UseRouting()
        .UseAuthorization()
        .UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
            endpoints.MapFallback(() => Results.Redirect("/swagger"));
        });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

public partial class Program { } // added to support integration tests