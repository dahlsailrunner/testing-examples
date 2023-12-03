using Microsoft.AspNetCore.Mvc.Testing;

namespace CarvedRock.Catalog.IntegrationTests.Utilities;

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