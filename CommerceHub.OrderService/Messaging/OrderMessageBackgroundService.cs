using CommerceHub.OrderService.Data;
using CommerceHub.OrderService.Messaging;
using CommerceHub.OrderService.Models;

public class OrderMessageBackgroundService : BackgroundService
{
    private readonly IOrderMessageQueue _orderMessageQueue;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderMessageBackgroundService(
        IOrderMessageQueue orderMessageQueue,
        IServiceScopeFactory scopeFactory)
    {
        _orderMessageQueue = orderMessageQueue;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await _orderMessageQueue
                .DequeueAsync(stoppingToken);

            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<OrderDbContext>();

            var audit = new OrderAudit
            {
                Id = Guid.NewGuid(),
                OrderId = message.OrderId,
                EventType = "OrderCreated",
                CreatedAt = DateTime.UtcNow
            };

            dbContext.OrderAudits.Add(audit);

            await dbContext.SaveChangesAsync(
                stoppingToken);
        }
    }
}