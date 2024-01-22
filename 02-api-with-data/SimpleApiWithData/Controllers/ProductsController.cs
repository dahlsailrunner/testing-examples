using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApiWithData.Data;
using SimpleApiWithData.Data.Entities;
using SimpleApiWithData.Validators;

namespace SimpleApiWithData.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class ProductsController(LocalContext context, ProductValidator validator) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Product>> GetProducts(string category = "all")
    {
        return await context.Products
            .Where(p => p.Category == category || category == "all")
            .ToListAsync();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<Product> Get(int id)
    {
        var product = await context.Products.FindAsync(id);
        return product ?? throw new KeyNotFoundException($"Product with id {id} not found");
    }

    [HttpPost]
    public async Task<Product> Post(Product product, CancellationToken token)
    {
        await validator.ValidateAndThrowAsync(product, token);
        await context.Products.AddAsync(product, token);
        await context.SaveChangesAsync(token);
        return product;
    }
}
