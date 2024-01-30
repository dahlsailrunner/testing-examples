namespace SimpleApiWithPostgresAndAuth.Tests.Utilities;

public class CustomApiFactory<TProgram>(DatabaseFixture dbFixture) : AnonymousApiFactory<TProgram>(dbFixture)
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // replace the default ExternalAPI URl with the WireMock server URL (original was in appsettings.json)
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ExternalApiBaseUrl"] = dbFixture.ExternalApiBaseUrlOverride
            }!);
        });

        builder.ConfigureServices(services => 
        {
            services.AddSingleton<IStartupFilter>(new AutoAuthorizeStartupFilter());
        });
    }
}