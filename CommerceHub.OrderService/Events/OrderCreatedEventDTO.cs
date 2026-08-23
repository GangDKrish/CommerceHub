namespace CommerceHub.OrderService.Events
{
    public class OrderCreatedEventDTO
    {
        public Guid OrderId { get; set; }

        public string CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
