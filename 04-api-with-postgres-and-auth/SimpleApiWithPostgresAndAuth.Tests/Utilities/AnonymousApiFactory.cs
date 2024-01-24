using Microsoft.AspNetCore.Identity;

namespace SimpleApiWithPostgresAndAuth.Tests.Utilities;

public class AnonymousApiFactory<TProgram>(DatabaseFixture dbFixture) : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("test");

        builder.SwapDatabase(dbFixture.TestConnectionString);

        builder.ConfigureServices(services =>
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.RoleClaimType = "role";
            }));
    }
}
