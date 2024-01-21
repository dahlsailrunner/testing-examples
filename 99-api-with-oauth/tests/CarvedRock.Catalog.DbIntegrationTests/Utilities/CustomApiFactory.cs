using Microsoft.AspNetCore.Mvc.Testing;

namespace CarvedRock.Catalog.DbIntegrationTests.Utilities;

public class CustomApiFactory<TProgram>(SharedFixture fixture) : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("test");
        builder.SwapDatabase(fixture.TestConnectionString);
    }
}