using Asp.Versioning.ApiExplorer;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CarvedRock.Catalog.Api.Swagger;

public class ConfigureSwaggerOptions(IConfiguration config, IApiVersionDescriptionProvider provider)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        var disco = GetDiscoveryDocument();

        var apiScope = config.GetValue<string>("Authentication:ApiName");
        var scopes = apiScope.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        var addScopes = config.GetValue<string>("Authentication:AdditionalScopes");
        var additionalScopes = addScopes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        scopes.AddRange(additionalScopes);

        var oauthScopeDic = scopes.ToDictionary(scope => scope, scope => $"Resource access: {scope}");

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Title = $"CarvedRock_Catalog {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                });
        }

        options.EnableAnnotations();

        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(disco.AuthorizeEndpoint),
                    TokenUrl = new Uri(disco.TokenEndpoint),
                    Scopes = oauthScopeDic
                }
            }
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                },
                oauthScopeDic.Keys.ToArray()
            }
        });
    }

    private DiscoveryDocumentResponse GetDiscoveryDocument()
    {
        var client = new HttpClient();
        var authority = config.GetValue<string>("Authentication:Authority");
        return client.GetDiscoveryDocumentAsync(authority)
            .GetAwaiter()
            .GetResult();
    }
}
