using Bogus;
using CarvedRock.Data;
using CarvedRock.Data.Entities;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace CarvedRock.Catalog.DbIntegrationTests.Utilities;

public class SharedFixture : IAsyncLifetime
{
    public const int ProductCount = 50;

    public string AccessToken { get; private set; } = string.Empty;
    
    public string TestConnectionString => _dbContainer.GetConnectionString();

    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("carved_rock_tests")
            .WithUsername("postgres")
            .WithPassword("notapassword")
            .Build();

    public async Task InitializeAsync()
    {
        await using var factory = new CustomApiFactory<Program>(this);

        await _dbContainer.StartAsync();

        var cfg = factory.Services.GetRequiredService<IConfiguration>();
        var authority = cfg.GetValue<string>("Authentication:Authority");
        var apiName = cfg.GetValue<string>("Authentication:ApiName");

        var tokenClient = new HttpClient();
        var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = $"{authority}/connect/token",
            Scope = apiName,
            ClientId = "m2m",
            ClientSecret = "secret"
        });
        AccessToken = tokenResponse.AccessToken!;

        var optionsBuilder = new DbContextOptionsBuilder<LocalContext>()
            .UseNpgsql(TestConnectionString);
        var dbContext = new LocalContext(optionsBuilder.Options);
        await dbContext.Database.MigrateAsync();
        await CreateBogusTestData(dbContext);
    }

    private static async Task CreateBogusTestData(LocalContext dbContext)
    {
        var ratingFaker = new Faker<ProductRating>()
            .RuleFor(r => r.AggregateRating, f => f.Random.Decimal(1, 5))
            .RuleFor(r => r.NumberOfRatings, f => f.Random.Int(1, 1000));

        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => double.Parse(f.Commerce.Price()))
            .RuleFor(p => p.ImgUrl, f => $"{f.Internet.Url()}/some_image.png")
            .RuleFor(p => p.Category, f => f.Commerce.ProductAdjective())
            .RuleFor(p=> p.Rating, f => ratingFaker.Generate().OrNull(f));

        var products = productFaker.Generate(ProductCount);
        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}

[CollectionDefinition("API Integration Test")]
public class SharedCollection : ICollectionFixture<SharedFixture>
{
    // This class has no code, and is never created. Its purpose is simply to be the place
    // to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
}