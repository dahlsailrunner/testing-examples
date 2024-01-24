using Microsoft.EntityFrameworkCore;
using SimpleApiWithPostgresAndAuth.Data.Entities;

namespace SimpleApiWithPostgresAndAuth.Data;

public class LocalContext(DbContextOptions<LocalContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; } = null!;
}
