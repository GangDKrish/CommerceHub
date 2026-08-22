using CommerceHub.ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.ProductService.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options)
    {
    }

    public DbSet<Products> Products => Set<Products>();
}