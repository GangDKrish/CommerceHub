namespace CommerceHub.OrderService.Services;

public interface IProductClient
{
    Task<ProductLookupResult> GetProductAsync(Guid productId);
}

public record ProductLookupResult(
    bool Found,
    decimal Price);
