namespace CommerceHub.OrderService.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderItem> Items { get; set; } = [];
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}
