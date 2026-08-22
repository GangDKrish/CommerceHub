using CommerceHub.InventoryService.Data;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.InventoryService.GrpcServices;

public class InventoryGrpcService : Inventory.InventoryBase
{
    private readonly InventoryDbContext _dbContext;

    public InventoryGrpcService(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<CheckAvailabilityResponse>
        CheckAvailability(
            CheckAvailabilityRequest request,
            Grpc.Core.ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
        {
            return new CheckAvailabilityResponse
            {
                IsAvailable = false,
                AvailableQuantity = 0
            };
        }

        var inventory = await _dbContext.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductId == productId);

        if (inventory == null)
        {
            return new CheckAvailabilityResponse
            {
                IsAvailable = false,
                AvailableQuantity = 0
            };
        }

        return new CheckAvailabilityResponse
        {
            IsAvailable =
                inventory.AvailableQuantity >= request.Quantity,

            AvailableQuantity =
                inventory.AvailableQuantity
        };
    }

    public override async Task<ReserveInventoryResponse>
        ReserveInventory(
            ReserveInventoryRequest request,
            Grpc.Core.ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
        {
            return new ReserveInventoryResponse
            {
                Success = false,
                AvailableQuantity = 0
            };
        }

        var inventory = await _dbContext.InventoryItems
            .FirstOrDefaultAsync(x => x.ProductId == productId);

        if (inventory == null)
        {
            return new ReserveInventoryResponse
            {
                Success = false,
                AvailableQuantity = 0
            };
        }

        if (request.Quantity <= 0 ||
            inventory.AvailableQuantity < request.Quantity)
        {
            return new ReserveInventoryResponse
            {
                Success = false,
                AvailableQuantity = inventory.AvailableQuantity
            };
        }

        inventory.AvailableQuantity -= request.Quantity;
        inventory.ReservedQuantity += request.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new ReserveInventoryResponse
        {
            Success = true,
            AvailableQuantity = inventory.AvailableQuantity
        };
    }

    public override async Task<ReleaseInventoryResponse>
        ReleaseInventory(
            ReleaseInventoryRequest request,
            Grpc.Core.ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
        {
            return new ReleaseInventoryResponse
            {
                Success = false
            };
        }

        if (request.Quantity <= 0)
        {
            return new ReleaseInventoryResponse
            {
                Success = false
            };
        }

        var inventory = await _dbContext.InventoryItems
            .FirstOrDefaultAsync(x => x.ProductId == productId);

        if (inventory == null ||
            inventory.ReservedQuantity < request.Quantity)
        {
            return new ReleaseInventoryResponse
            {
                Success = false
            };
        }

        inventory.ReservedQuantity -= request.Quantity;
        inventory.AvailableQuantity += request.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new ReleaseInventoryResponse
        {
            Success = true,
            AvailableQuantity = inventory.AvailableQuantity,
            ReservedQuantity = inventory.ReservedQuantity
        };
    }
}