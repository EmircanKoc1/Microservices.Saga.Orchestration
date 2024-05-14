using MassTransit;
using Shared;
using Shared.PaymentEvents;

namespace Payment.API.Consumers
{
    public class PaymentStartedEventConsumer(ISendEndpointProvider _sendEndpointProvider) : IConsumer<PaymentStartedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentStartedEvent> context)
        {
            var sendEnpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));



            if (true)
            {
                PaymentCompletedEvent paymentCompletedEvent = new(context.Message.CorrelationId)
                {

                };
               await sendEnpoint.Send<PaymentCompletedEvent>(paymentCompletedEvent);
            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new(context.Message.CorrelationId)
                {
                    Message = "yetersiz bakiye",
                    OrderItems = context.Message.OrderItems
                };

                await sendEnpoint.Send<PaymentFailedEvent>(paymentFailedEvent);

            }
        }
    }
}
