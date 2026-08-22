using CommerceHub.InventoryService;

namespace CommerceHub.OrderService.Services;

public class InventoryGrpcClient : IInventoryClient
{
    private readonly Inventory.InventoryClient _client;

    public InventoryGrpcClient(
        Inventory.InventoryClient client)
    {
        _client = client;
    }

    public async Task<InventoryReservationResult>
        ReserveInventoryAsync(
            Guid productId,
            int quantity)
    {
        var response = await _client.ReserveInventoryAsync(
            new ReserveInventoryRequest
            {
                ProductId = productId.ToString(),
                Quantity = quantity
            });

        return new InventoryReservationResult(
            response.Success,
            response.AvailableQuantity,
            response.ReservedQuantity);
    }

    public async Task<InventoryReservationResult>
        ReleaseInventoryAsync(
            Guid productId,
            int quantity)
    {
        var response = await _client.ReleaseInventoryAsync(
            new ReleaseInventoryRequest
            {
                ProductId = productId.ToString(),
                Quantity = quantity
            });

        return new InventoryReservationResult(
            response.Success,
            response.AvailableQuantity,
            response.ReservedQuantity);
    }
}