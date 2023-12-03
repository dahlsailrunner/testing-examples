namespace CarvedRock.Catalog.Api.StartupServices;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options => 
                { options.ReportApiVersions = true; })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
