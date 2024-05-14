using MassTransit;

namespace Shared.PaymentEvents
{
    public class PaymentCompletedEvent : CorrelatedBy<Guid>
    {
        public PaymentCompletedEvent(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; }
    }
}
