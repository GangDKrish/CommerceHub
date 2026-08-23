using CommerceHub.OrderService.Events;
using CommerceHub.OrderService.Messaging;
using System.Threading.Channels;

public class OrderMessageQueue : IOrderMessageQueue
{
    private readonly Channel<OrderCreatedEventDTO> _channel;

    public OrderMessageQueue()
    {
        _channel = Channel.CreateUnbounded<OrderCreatedEventDTO>();
    }

    public ValueTask EnqueueAsync(OrderCreatedEventDTO message, CancellationToken cancellationToken = default)
    {
        return _channel.Writer.WriteAsync(message, cancellationToken);
    }

    public ValueTask<OrderCreatedEventDTO> DequeueAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAsync(cancellationToken);
    }
}