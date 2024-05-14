using MassTransit;

namespace Shared.StockEvents
{
    public class StockNotReservedEvent : CorrelatedBy<Guid>
    {
        public StockNotReservedEvent(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; }
        public string Message { get; set; }
    }
}
