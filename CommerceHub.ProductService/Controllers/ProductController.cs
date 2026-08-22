using CommerceHub.ProductService.Data;
using CommerceHub.ProductService.DTOs;
using CommerceHub.ProductService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.ProductService.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ProductDbContext _dbContext;

    public ProductController(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetProducts()
    {
        var products = await _dbContext.Products
            .AsNoTracking()
            .Select(product => new ProductResponseDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Category = product.Category
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponseDTO>> GetProduct(Guid id)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return NotFound();

        return Ok(new ProductResponseDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDTO>> CreateProduct(
        CreateProductRequestDTO request)
    {
        var product = new Products
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync();

        var response = new ProductResponseDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category
        };

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            response);
    }
}