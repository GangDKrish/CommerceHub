namespace CommerceHub.OrderService.Models
{
    public class InventoryReservation
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
