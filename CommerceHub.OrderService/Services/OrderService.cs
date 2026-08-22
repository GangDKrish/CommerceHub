using CommerceHub.OrderService.Data;
using CommerceHub.OrderService.DTOs;
using CommerceHub.OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.OrderService.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _dbContext;
    private readonly IInventoryClient _inventoryClient;

    public OrderService(
        OrderDbContext dbContext,
        IInventoryClient inventoryClient)
    {
        _dbContext = dbContext;
        _inventoryClient = inventoryClient;
    }

    public async Task<OrderResponseDTO> CreateOrderAsync(
        CreateOrderRequestDTO request)
    {
        var reservedItems =
            new List<CreateOrderItemRequestDTO>();
        // check the availability
        foreach (var item in request.Items)
        {
            var reservation = await _inventoryClient
                .ReserveInventoryAsync(
                    item.ProductId,
                    item.Quantity);

            if (!reservation.Success)
            {
                throw new InsufficientInventoryException(
                    item.ProductId,
                    reservation.AvailableQuantity);
            }
            reservedItems.Add(item);
        }

        var totalAmount = reservedItems.Sum(item => item.UnitPrice * item.Quantity);

        // create order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            TotalAmount = totalAmount,
            Status = OrderStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        // create order items
        foreach (var item in request.Items)
        {
            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.UnitPrice * item.Quantity
            });
        }

        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();

        return new OrderResponseDTO
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
    }
}