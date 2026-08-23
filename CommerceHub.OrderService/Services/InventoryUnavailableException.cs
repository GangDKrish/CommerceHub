namespace CommerceHub.OrderService.Services;

public class InventoryUnavailableException : Exception
{
    public InventoryUnavailableException(Exception innerException)
        : base(
            "The inventory service is currently unavailable. " +
            "Please try again later.",
            innerException)
    {
    }
}
