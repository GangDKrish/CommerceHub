namespace CommerceHub.InventoryService.DTOs
{
    public class InventoryResponseDTO
    {
        public Guid ProductId { get; set; }

        public int AvailableQuantity { get; set; }

        public int ReservedQuantity { get; set; } 
    }
}
