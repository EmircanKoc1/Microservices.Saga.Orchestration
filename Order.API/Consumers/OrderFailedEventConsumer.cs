using MassTransit;
using Order.API.Context;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
    public class OrderFailedEventConsumer(OrderDbContext _context) : IConsumer<OrderFailedEvent>
    {
        public async Task Consume(ConsumeContext<OrderFailedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);

            if (order is not null)
            {
                order.OrderStatus = Enums.OrderStatus.Fail;

                await _context.SaveChangesAsync();
            }

        }
    }
}
