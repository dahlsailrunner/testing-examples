using CarvedRock.Data;
using Microsoft.EntityFrameworkCore;

namespace CarvedRock.Catalog.DbIntegrationTests.Utilities;

public static class WebHostExtensions
{
    public static IWebHostBuilder SwapDatabase(this IWebHostBuilder builder, string connectionString)
    {
        return builder.ConfigureServices(services =>
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
                options.UseNpgsql(connectionString);
                options.EnableSensitiveDataLogging();
            });
        });
    }
}
