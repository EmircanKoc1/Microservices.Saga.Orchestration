using MassTransit;
using Shared.Messages;

namespace Shared.OrderEvents
{
    public class OrderCreatedEvent : CorrelatedBy<Guid>
    {
        public OrderCreatedEvent(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; }

        public List<OrderItemMessage> OrderItems { get; set; }

    }
}
