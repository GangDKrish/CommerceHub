using CommerceHub.OrderService.Data;
using CommerceHub.OrderService.DTOs;
using CommerceHub.OrderService.Events;
using CommerceHub.OrderService.Messaging;
using CommerceHub.OrderService.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace CommerceHub.OrderService.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _dbContext;
    private readonly IInventoryClient _inventoryClient;
    private readonly IProductClient _productClient;
    private readonly IOrderMessageQueue _orderMessageQueue;

    public OrderService(OrderDbContext dbContext, IInventoryClient inventoryClient, IProductClient productClient, IOrderMessageQueue orderMessageQueue)
    {
        _dbContext = dbContext;
        _inventoryClient = inventoryClient;
        _productClient = productClient;
        _orderMessageQueue = orderMessageQueue;
    }

    public async Task<OrderResponseDTO> CreateOrderAsync(CreateOrderRequestDTO request)
    {
        // Resolve unit prices from the Product service (source of truth)
        var unitPrices = new Dictionary<Guid, decimal>();

        foreach (var item in request.Items)
        {
            if (unitPrices.ContainsKey(item.ProductId))
                continue;

            var product = await _productClient.GetProductAsync(item.ProductId);

            if (!product.Found)
                throw new ProductNotFoundException(item.ProductId);

            unitPrices[item.ProductId] = product.Price;
        }

        var reservedStock = new List<InventoryReservation>();

        // Reserve inventory for every item
        foreach (var item in request.Items)
        {
            InventoryReservationResult reservation;

            try
            {
                reservation = await _inventoryClient
                    .ReserveInventoryAsync(
                        item.ProductId,
                        item.Quantity);
            }
            catch (RpcException ex)
            {
                await ReleaseReservedStockAsync(reservedStock);

                throw new InventoryUnavailableException(ex);
            }

            // Reservation failed
            if (!reservation.Success)
            {
                // Undo all previous successful reservations
                await ReleaseReservedStockAsync(reservedStock);

                throw new InsufficientInventoryException(
                    item.ProductId,
                    reservation.AvailableQuantity);
            }

            // successful reserved stocks
            reservedStock.Add(new InventoryReservation
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }


        var totalAmount = request.Items.Sum(item => unitPrices[item.ProductId] * item.Quantity);

        // Create Order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            TotalAmount = totalAmount,
            Status = OrderStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        // Create OrderItems
        foreach (var item in request.Items)
        {
            var unitPrice = unitPrices[item.ProductId];

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = unitPrice * item.Quantity
            });
        }

        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();

        var orderCreatedEvent = new OrderCreatedEventDTO
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt
        };

        await _orderMessageQueue.EnqueueAsync(orderCreatedEvent);

        return new OrderResponseDTO
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt
        };
    }

    private async Task ReleaseReservedStockAsync(IEnumerable<InventoryReservation> reservations)
    {
        foreach (var reservation in reservations)
        {
                await _inventoryClient.ReleaseInventoryAsync(
                    reservation.ProductId,
                    reservation.Quantity);
        }
    }
}