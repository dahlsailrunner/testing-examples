using IdentityModel.Client;

namespace CarvedRock.Catalog.DbIntegrationTests.Utilities;

public class SharedFixture : IAsyncLifetime
{
    public string AccessToken { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        await using var factory = new CustomApiFactory<Program>();

        var cfg = factory.Services.GetRequiredService<IConfiguration>();
        var authority = cfg.GetValue<string>("Authentication:Authority");
        var apiName = cfg.GetValue<string>("Authentication:ApiName");

        var tokenClient = new HttpClient();
        var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = $"{authority}/connect/token",
            Scope = apiName,
            ClientId = "m2m",
            ClientSecret = "secret"
        });

        AccessToken = tokenResponse.AccessToken!;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

[CollectionDefinition("API Integration Test")]
public class SharedCollection : ICollectionFixture<SharedFixture>
{
    // This class has no code, and is never created. Its purpose is simply to be the place
    // to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
}