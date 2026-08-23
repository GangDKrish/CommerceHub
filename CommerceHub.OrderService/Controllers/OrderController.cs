using CommerceHub.OrderService.Data;
using CommerceHub.OrderService.DTOs;
using CommerceHub.OrderService.Models;
using CommerceHub.OrderService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _dbContext;

    private readonly IOrderService _orderService;

    public OrderController(OrderDbContext dbContext,IOrderService orderService)
    {
        _dbContext = dbContext;
        _orderService = orderService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponseDTO>> GetOrder(Guid id)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
            return NotFound();

        return Ok(new OrderResponseDTO
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrders()
    {
        var orders = await _dbContext.Orders
            .AsNoTracking()
            .Select(order => new OrderResponseDTO()
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDTO>> CreateOrder(
        CreateOrderRequestDTO request)
    {
        if (request.Items == null || request.Items.Count == 0)
            return BadRequest("Order must contain at least one item.");

        try
        {
            var order = await _orderService.CreateOrderAsync(request);

            return Ok(order);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InsufficientInventoryException ex)
        {
            return Conflict(ex.Message);
        }
        catch (InventoryUnavailableException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
    }
}