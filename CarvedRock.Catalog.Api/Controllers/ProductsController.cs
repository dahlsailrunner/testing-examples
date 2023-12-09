using Asp.Versioning;
using CarvedRock.Data;
using CarvedRock.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarvedRock.Catalog.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class ProductsController(LocalContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Product>> Get()
    {
        return await dbContext.Products.ToListAsync();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<Product> Get(int id)
    {
        var product = await dbContext.Products.FindAsync(id);
        return product ?? throw new KeyNotFoundException($"Product with id {id} not found");
    }


}
