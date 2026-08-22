namespace CommerceHub.OrderService.Services;

public interface IInventoryClient
{
    Task<InventoryReservationResult> ReserveInventoryAsync(
        Guid productId,
        int quantity);
    Task<InventoryReservationResult> ReleaseInventoryAsync(
        Guid productId,
        int quantity);
}

public record InventoryReservationResult(
    bool Success,
    int AvailableQuantity,
    int ReservedQuantity);

