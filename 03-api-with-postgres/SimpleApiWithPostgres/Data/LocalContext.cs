using Microsoft.EntityFrameworkCore;
using SimpleApiWithPostgres.Data.Entities;

namespace SimpleApiWithPostgres.Data;

public class LocalContext(DbContextOptions<LocalContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; } = null!;
}
