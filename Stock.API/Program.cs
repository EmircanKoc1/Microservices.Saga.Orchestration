using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Messages;
using Shared.OrderEvents;
using Stock.API.Consumers;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.AddConsumer<StockRollBackMessageConsumer>();


    configurator.UsingRabbitMq((_context, _configurator) =>
    {
        _configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue,e=>e.ConfigureConsumer<OrderCreatedEventConsumer>(_context));

        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_RollbackMessageQueue, e => e.ConfigureConsumer<StockRollBackMessageConsumer>(_context));





    });



});
builder.Services.AddSingleton<MongoDbService>();


var app = builder.Build();

using var scope = builder.Services.BuildServiceProvider().CreateScope();
var mongoDbService = scope.ServiceProvider.GetRequiredService<MongoDbService>();


if (!await (await mongoDbService.GetCollection<Stock.API.Models.Stock>().FindAsync(x => true)).AnyAsync())
{
    await mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOneAsync(new()
    {
        ProductId = 1,
        Count = 200
    });

    await mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOneAsync(new()
    {
        ProductId = 2,
        Count = 300
    });

    await mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOneAsync(new()
    {
        ProductId = 3,
        Count = 50
    });

    await mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOneAsync(new()
    {
        ProductId = 4,
        Count = 10
    });

    await mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOneAsync(new()
    {
        ProductId = 5,
        Count = 500
    });


}

app.Run();

