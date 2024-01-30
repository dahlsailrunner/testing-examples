using System.Text.Json;
using Bogus;
using Microsoft.EntityFrameworkCore;
using SimpleApiWithPostgresAndAuth.Data;
using Testcontainers.PostgreSql;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SimpleApiWithPostgresAndAuth.Tests.Utilities;

public class DatabaseFixture :IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("simple_api_tests")
            .WithUsername("postgres")
            .WithPassword("notapassword")
            .Build();

    public string ExternalApiBaseUrlOverride { get; private set; } = null!;
    public string TestConnectionString => _dbContainer.GetConnectionString();

    private LocalContext? _dbContext;

    public List<Product> OriginalProducts = [];
    public Faker<Product> ProductFaker { get; } = new Faker<Product>()
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.Price, f => Convert.ToDouble(f.Commerce.Price()))
        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
        .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
        .RuleFor(p => p.ImgUrl, f => f.Image.PicsumUrl());

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<LocalContext>()
            .UseNpgsql(TestConnectionString);
        _dbContext = new LocalContext(optionsBuilder.Options);

        await _dbContext.Database.MigrateAsync();

        await CreateFreshSampleData(100);

        OriginalProducts = await _dbContext.Products.ToListAsync();

        var server = WireMockServer.Start();

        var claims = new List<InternalClaim>
        {
            new("email", "hi@there.com"),
            new("role", "admin"),
            new("sub", "1234567890")
        };

        ExternalApiBaseUrlOverride = server.Url!;
        server
            .Given(
                Request.Create().WithPath("/api/test").UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(202)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(claims, new JsonSerializerOptions(JsonSerializerDefaults.Web)))
            );
    }

    private async Task CreateFreshSampleData(int numberOfProductsToCreate)
    {
        var products = ProductFaker.Generate(numberOfProductsToCreate);
        _dbContext!.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
        }
    }
}

[CollectionDefinition("IntegrationTests")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply to be the place
    // to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
}