using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarvedRock.Data;

public static class Initialization
{
    public static void InitializeDatabase(this IServiceProvider serviceProvider, string environmentName)
    {
        using var scope = serviceProvider.CreateScope(); // required because dbContext is a scoped service
        using var context = new LocalContext(scope.ServiceProvider.GetRequiredService<DbContextOptions<LocalContext>>());
        context.Database.EnsureCreated();
        context.Database.Migrate();

        if (environmentName == "Development")
        {
            context.StartWithSampleData();
        }
    }
}
