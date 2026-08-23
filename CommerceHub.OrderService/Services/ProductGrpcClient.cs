using System.Globalization;
using CommerceHub.ProductService;

namespace CommerceHub.OrderService.Services;

public class ProductGrpcClient : IProductClient
{
    private readonly Product.ProductClient _client;

    public ProductGrpcClient(
        Product.ProductClient client)
    {
        _client = client;
    }

    public async Task<ProductLookupResult> GetProductAsync(Guid productId)
    {
        var response = await _client.GetProductAsync(
            new GetProductRequest
            {
                ProductId = productId.ToString()
            });

        if (!response.Found)
        {
            return new ProductLookupResult(false, 0m);
        }

        var price = decimal.Parse(
            response.Price,
            NumberStyles.Number,
            CultureInfo.InvariantCulture);

        return new ProductLookupResult(true, price);
    }
}
