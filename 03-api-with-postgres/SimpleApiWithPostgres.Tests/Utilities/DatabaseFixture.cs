using Bogus;
using Microsoft.EntityFrameworkCore;
using SimpleApiWithPostgres.Data;
using SimpleApiWithPostgres.Data.Entities;
using Testcontainers.PostgreSql;

namespace SimpleApiWithPostgres.Tests.Utilities;

public class DatabaseFixture :IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithDatabase("simple_api_tests")
            .WithUsername("postgres")
            .WithPassword("notapassword")
            .Build();

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