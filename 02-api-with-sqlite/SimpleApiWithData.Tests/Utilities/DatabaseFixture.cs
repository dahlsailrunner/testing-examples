using Bogus;
using Microsoft.EntityFrameworkCore;
using SimpleApiWithData.Data;
using SimpleApiWithData.Data.Entities;

namespace SimpleApiWithData.Tests.Utilities;

public class DatabaseFixture :IAsyncLifetime
{
    public const string DatabaseName = "InMemTestDb;Mode=Memory;Cache=Shared;";
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
        var options = new DbContextOptionsBuilder<LocalContext>()
            .UseSqlite($"Data Source={DatabaseName}")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        _dbContext = new LocalContext(options);

        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
        await _dbContext.Database.OpenConnectionAsync();
        await _dbContext.Database.MigrateAsync();

        CreateFreshSampleData(100);

        OriginalProducts = await _dbContext.Products.ToListAsync();
    }

    private void CreateFreshSampleData(int numberOfProductsToCreate)
    {
        var products = ProductFaker.Generate(numberOfProductsToCreate);
        _dbContext!.Products.AddRange(products);
        _dbContext.SaveChanges();
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