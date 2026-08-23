using CommerceHub.OrderService.Events;

namespace CommerceHub.OrderService.Messaging
{
    public interface IOrderMessageQueue
    {
        ValueTask EnqueueAsync(OrderCreatedEventDTO message, CancellationToken cancellationToken = default);

        ValueTask<OrderCreatedEventDTO> DequeueAsync(CancellationToken cancellationToken = default);
    }
}
