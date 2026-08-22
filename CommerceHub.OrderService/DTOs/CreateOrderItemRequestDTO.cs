namespace CommerceHub.OrderService.DTOs
{
    public class CreateOrderItemRequestDTO
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
