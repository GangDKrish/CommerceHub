namespace CommerceHub.OrderService.Services;

public class InsufficientInventoryException : Exception
{
    public InsufficientInventoryException(
        Guid productId,
        int availableQuantity)
        : base(
            $"Insufficient inventory for product {productId}. " +
            $"Available quantity: {availableQuantity}.")
    {
    }
}