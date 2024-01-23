using Microsoft.EntityFrameworkCore;
using SimpleApiWithData.Data;

namespace SimpleApiWithData.Tests.Utilities;

public class CustomApiFactory<TProgram> : WebApplicationFactory<TProgram>
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
            services.AddDbContext<LocalContext>(opts =>
                opts.UseSqlite($"Data Source={DatabaseFixture.DatabaseName}")
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        });
    }
}