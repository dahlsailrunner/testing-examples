using Microsoft.EntityFrameworkCore;
using SimpleApiWithPostgres.Data;

namespace SimpleApiWithPostgres.Tests.Utilities;

public class CustomApiFactory<TProgram>(DatabaseFixture dbFixture) : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("test");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LocalContext>));
            services.Remove(dbContextDescriptor!);

            var ctx = services.SingleOrDefault(d => d.ServiceType == typeof(LocalContext));
            services.Remove(ctx!);

            // add back the container-based dbContext
            services.AddDbContext<LocalContext>(options =>
                options.UseNpgsql(dbFixture.TestConnectionString));
        });
    }
}