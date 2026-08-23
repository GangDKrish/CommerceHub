namespace CommerceHub.OrderService.Services;

public class ProductNotFoundException : Exception
{
    public Guid ProductId { get; }

    public ProductNotFoundException(Guid productId)
        : base($"Product {productId} was not found.")
    {
        ProductId = productId;
    }
}
