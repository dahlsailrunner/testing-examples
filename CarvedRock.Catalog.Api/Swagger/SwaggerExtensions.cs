using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CarvedRock.Catalog.Api.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerFeatures(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen();

        return services;
    }

    public static IApplicationBuilder UseSwaggerFeatures(this IApplicationBuilder app, IConfiguration config,
        IApiVersionDescriptionProvider provider, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            return app;
        }

        var clientId = config.GetValue<string>("Authentication:SwaggerClientId");
        app
            .UseSwagger()
            .UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        $"CarvedRock_Catalog API {description.GroupName.ToUpperInvariant()}");
                    options.RoutePrefix = string.Empty;
                }

                options.DocumentTitle = "CarvedRock Catalog Documentation";
                options.OAuthClientId(clientId);
                options.OAuthAppName("CarvedRock_Catalog");
                options.OAuthUsePkce();
            });

        return app;
    }
}
