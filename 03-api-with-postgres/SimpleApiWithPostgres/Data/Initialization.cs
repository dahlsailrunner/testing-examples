using Bogus;
using Microsoft.EntityFrameworkCore;
using SimpleApiWithPostgres.Data.Entities;

namespace SimpleApiWithPostgres.Data;

public static class Initialization
{
    public static void InitializeDatabase(this IServiceProvider serviceProvider, string environmentName)
    {
        using var scope = serviceProvider.CreateScope(); // required because dbContext is a scoped service
        using var context = new LocalContext(scope.ServiceProvider.GetRequiredService<DbContextOptions<LocalContext>>());

        if (environmentName == "Development")
        {
            StartWithFreshDevData(context);
        }
        else
        {
            context.Database.Migrate(); // just make sure we're up-to-date
        }
    }

    private static void StartWithFreshDevData(LocalContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.Migrate();

        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => Convert.ToDouble(f.Commerce.Price()))
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(p => p.ImgUrl, f => f.Image.PicsumUrl());

        context.Products.AddRange(productFaker.Generate(30));
        //context.Products.Add(new Product
        //{
        //    Name = "Trailblazer",
        //    Category = "boots",
        //    Price = 69.99,
        //    Description = "Great support in this high-top to take you to great heights and trails.",
        //    ImgUrl = "https://www.pluralsight.com/content/dam/pluralsight2/teach/author-tools/carved-rock-fitness/img-brownboots.jpg",
        //});
        //context.Products.Add(new Product
        //{
        //    Name = "Coastliner",
        //    Category = "boots",
        //    Price = 49.99,
        //    Description =
        //        "Easy in and out with this lightweight but rugged shoe with great ventilation to get your around shores, beaches, and boats.",
        //    ImgUrl = "https://www.pluralsight.com/content/dam/pluralsight2/teach/author-tools/carved-rock-fitness/img-greyboots.jpg",
        //});
        //context.Products.Add(new Product
        //{
        //    Name = "Woodsman",
        //    Category = "boots",
        //    Price = 64.99,
        //    Description =
        //        "All the insulation and support you need when wandering the rugged trails of the woods and backcountry.",
        //    ImgUrl = "/images/shutterstock_222721876.jpg",
        //});
        //context.Products.Add(new Product
        //{
        //    Name = "Billy",
        //    Category = "boots",
        //    Price = 79.99,
        //    Description =
        //        "Get up and down rocky terrain like a billy-goat with these awesome high-top boots with outstanding support.",
        //    ImgUrl = "/images/shutterstock_475046062.jpg"
        //});
        //context.Products.Add(new Product
        //{
        //    Name = "Sherpa",
        //    Category = "equip",
        //    Price = 129.99,
        //    Description =
        //        "Manage and carry your gear with ease using this backpack with great lumbar support.",
        //    ImgUrl = "/images/shutterstock_6170527.jpg"
        //});
        //context.Products.Add(new Product
        //{
        //    Name = "Glide",
        //    Category = "kayak",
        //    Price = 399.99,
        //    Description =
        //        "Navigate tricky waterways easily with this great and manageable kayak.",
        //    ImgUrl = "/images/shutterstock_645036007.jpg"
        //});

        context.SaveChanges();
    }
}
