using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarvedRock.Data;

public static class Initialization
{
    public static void InitializeDatabase(this IServiceProvider serviceProvider, string environmentName)
    {
        using var scope = serviceProvider.CreateScope(); // required because dbContext is a scoped service
        using var context = new LocalContext(scope.ServiceProvider.GetRequiredService<DbContextOptions<LocalContext>>());

        if (environmentName == "Development")
        {
            context.StartWithSampleData();  // create / migrate database then add data
        }
        else
        {
            context.Database.Migrate(); // just make sure we're up-to-date
        }
    }
}
