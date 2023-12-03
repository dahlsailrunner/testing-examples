using Microsoft.AspNetCore.Mvc.Testing;

namespace CarvedRock.Catalog.DbIntegrationTests.Utilities;

public class CustomApiFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // builder.ConfigureServices(services =>
        // {
        // });
    }
}