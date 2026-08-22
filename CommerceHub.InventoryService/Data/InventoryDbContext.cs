using CommerceHub.InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.InventoryService.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(
        DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
}