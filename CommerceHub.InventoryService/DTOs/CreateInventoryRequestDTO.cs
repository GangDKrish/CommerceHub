namespace CommerceHub.InventoryService.DTOs
{
    public class CreateInventoryRequestDTO
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
