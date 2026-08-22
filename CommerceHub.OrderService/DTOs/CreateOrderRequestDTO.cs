namespace CommerceHub.OrderService.DTOs
{
    public class CreateOrderRequestDTO
    {
        public string CustomerId { get; set; } = string.Empty;

        public List<CreateOrderItemRequestDTO> Items { get; set; } = [];
    }
}
