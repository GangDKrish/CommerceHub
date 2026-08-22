using CommerceHub.InventoryService.Data;
using CommerceHub.InventoryService.DTOs;
using CommerceHub.InventoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.InventoryService.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly InventoryDbContext _dbContext;

    public InventoryController(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<InventoryResponseDTO>> GetInventory(
        Guid productId)
    {
        var inventory = await _dbContext.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductId == productId);

        if (inventory == null)
            return NotFound();

        return Ok(new InventoryResponseDTO
        {
            ProductId = inventory.ProductId,
            AvailableQuantity = inventory.AvailableQuantity,
            ReservedQuantity = inventory.ReservedQuantity
        });
    }

    [HttpPost]
    public async Task<ActionResult<InventoryResponseDTO>> CreateInventory(
        CreateInventoryRequestDTO request)
    {
        var existingInventory =
            await _dbContext.InventoryItems
                .FirstOrDefaultAsync(
                    x => x.ProductId == request.ProductId);

        if (existingInventory != null)
            return Conflict("Inventory already exists for this product.");

        if (request.Quantity < 0)
            return BadRequest("Quantity cannot be negative.");

        var inventory = new InventoryItem
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            AvailableQuantity = request.Quantity,
            ReservedQuantity = 0,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.InventoryItems.Add(inventory);

        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetInventory),
            new { productId = inventory.ProductId },
            new InventoryResponseDTO
            {
                ProductId = inventory.ProductId,
                AvailableQuantity = inventory.AvailableQuantity,
                ReservedQuantity = inventory.ReservedQuantity
            });
    }
}