using Microsoft.EntityFrameworkCore;
using SimpleApiWithData.Data.Entities;

namespace SimpleApiWithData.Data;

public class LocalContext(DbContextOptions<LocalContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; } = null!;
}
