
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachine.Service.StateDbContexts;
using SagaStateMachine.Service.StateInstances;
using SagaStateMachine.Service.StateMachine;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddMassTransit(configurator =>
{
    configurator.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
    .EntityFrameworkRepository(options =>
    {
        options.AddDbContext<DbContext, OrderStateDbContext>((provider, _builder) =>
        {
            _builder.UseSqlServer(builder.Configuration["SqlServer"]);
        });

    });


    configurator.UsingRabbitMq((_context, _configurator) =>
    {
        _configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));



    });



});


var host = builder.Build();



host.Run();
