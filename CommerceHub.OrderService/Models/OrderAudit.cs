namespace CommerceHub.OrderService.Models
{
    public class OrderAudit
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
