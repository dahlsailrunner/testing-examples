using Microsoft.EntityFrameworkCore;
using SimpleApiWithPostgresAndAuth.Data;

namespace SimpleApiWithPostgresAndAuth.Tests.Utilities;

public static class ApiFactoryExtensions
{
    public static IWebHostBuilder SwapDatabase(this IWebHostBuilder builder, string connectionString)
    {
        return builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LocalContext>));
            services.Remove(dbContextDescriptor!);

            var ctx = services.SingleOrDefault(d => d.ServiceType == typeof(LocalContext));
            services.Remove(ctx!);

            // add back the container-based dbContext
            services.AddDbContext<LocalContext>(options =>
                options.UseNpgsql(connectionString));
        });
    }
}
