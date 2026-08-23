using System.Globalization;
using CommerceHub.ProductService.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.ProductService.Services;

public class ProductGrpcService : Product.ProductBase
{
    private readonly ProductDbContext _dbContext;

    public ProductGrpcService(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GetProductResponse> GetProduct(
        GetProductRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
        {
            return new GetProductResponse { Found = false };
        }

        var product = await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == productId);

        if (product == null)
        {
            return new GetProductResponse { Found = false };
        }

        return new GetProductResponse
        {
            Found = true,
            ProductId = product.Id.ToString(),
            Name = product.Name,
            Price = product.Price.ToString(CultureInfo.InvariantCulture)
        };
    }
}
