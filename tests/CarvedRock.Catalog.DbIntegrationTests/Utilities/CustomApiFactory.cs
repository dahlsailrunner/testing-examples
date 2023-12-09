using CarvedRock.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace CarvedRock.Catalog.DbIntegrationTests.Utilities;

public class CustomApiFactory<TProgram>(SharedFixture fixture) : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("test");

        builder.ConfigureServices(services =>
        {
            // remove the default DbContext registrations
            var dbContextInterfaceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(LocalContext));
            services.Remove(dbContextInterfaceDescriptor!);

            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LocalContext>));
            services.Remove(dbContextDescriptor!);

            // add back the container-based dbContext
            services.AddDbContext<LocalContext>(options =>
            {
                options.UseNpgsql(fixture.TestConnectionString);
                options.EnableSensitiveDataLogging();
            });
        });
    }
}