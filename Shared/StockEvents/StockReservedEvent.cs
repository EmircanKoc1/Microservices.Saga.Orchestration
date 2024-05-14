using MassTransit;
using Shared.Messages;

namespace Shared.StockEvents
{
    public class StockReservedEvent : CorrelatedBy<Guid>
    {

        public StockReservedEvent(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; }
        public List<OrderItemMessage> OrderItems { get; set; }


    }



}
