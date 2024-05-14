
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Context;
using Order.API.Enums;
using Order.API.ViewModels;
using Shared;
using Shared.OrderEvents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});



builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCompletedEventConsumer>();
    configurator.AddConsumer<OrderFailedEventConsumer>();


    configurator.UsingRabbitMq((_context, _configurator) =>
    {
        _configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        _configurator.ReceiveEndpoint(RabbitMQSettings.Order_OrderCompletedEventQueue, e => e.ConfigureConsumer<OrderCompletedEventConsumer>(_context));
        _configurator.ReceiveEndpoint(RabbitMQSettings.Order_OrderFailedEventQueue, e => e.ConfigureConsumer<OrderFailedEventConsumer>(_context));




    });



});


var app = builder.Build();

app.UseSwaggerUI();
app.UseSwagger();

app.MapPost("create-order", async (CreateOrderViewModel model, OrderDbContext _context, ISendEndpointProvider sendEndpointProvider) =>
{

    var order = new Order.API.Models.Order()
    {
        BuyerId = model.BuyerId,
        CreatedDate = DateTime.UtcNow,
        OrderStatus = OrderStatus.Suspend,
        TotalPrice = model.OrderItems.Sum(oi => oi.Count * oi.Price),
        OrderItems = model.OrderItems.Select(x => new Order.API.Models.OrderItem
        {
            Price = x.Price,
            Count = x.Count,
            ProductId = x.ProductId
        }).ToList(),

    };

    await _context.Orders.AddAsync(order);
    await _context.SaveChangesAsync();

    OrderStartedEvent orderStartedEvent = new OrderStartedEvent()
    {
        BuyerId = model.BuyerId,
        OrderId = order.Id,
        TotalPrice = model.OrderItems.Sum(x => x.Count * x.Price),
        OrderItems = model.OrderItems.Select(x => new Shared.Messages.OrderItemMessage
        {
            Count = x.Count,
            Price = x.Price,
            ProductId = x.ProductId
        }).ToList()
    };

    ISendEndpoint sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));

    await sendEndpoint.Send<OrderStartedEvent>(orderStartedEvent);


});



app.Run();
