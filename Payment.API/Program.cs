using MassTransit;
using Payment.API.Consumers;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configurator =>
{

    configurator.AddConsumer<PaymentStartedEventConsumer>();

    configurator.UsingRabbitMq((_context, _configurator) =>
    {
        _configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        _configurator.ReceiveEndpoint(RabbitMQSettings.Payment_StartedEventQueue, e => e.ConfigureConsumer<PaymentStartedEventConsumer>(_context));




    });



});

var app = builder.Build();

app.Run();
