namespace SimpleApiWithPostgresAndAuth.Tests.Utilities;

public class CustomApiFactory<TProgram>(DatabaseFixture dbFixture) : AnonymousApiFactory<TProgram>(dbFixture)
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services => 
        {
            services.AddSingleton<IStartupFilter>(new AutoAuthorizeStartupFilter());
        });
    }
}