using CommerceHub.OrderService.DTOs;

namespace CommerceHub.OrderService.Services;

public interface IOrderService
{
    Task<OrderResponseDTO> CreateOrderAsync(
        CreateOrderRequestDTO request);
}