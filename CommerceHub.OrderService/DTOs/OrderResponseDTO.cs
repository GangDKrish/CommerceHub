using CommerceHub.OrderService.Models;

namespace CommerceHub.OrderService.DTOs
{
    public class OrderResponseDTO
    {
        public Guid Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
