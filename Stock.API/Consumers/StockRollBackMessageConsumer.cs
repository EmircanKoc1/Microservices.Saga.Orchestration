using MassTransit;
using MongoDB.Driver;
using Shared.Messages;
using Shared.PaymentEvents;
using Stock.API.Services;
using System.Linq.Expressions;

namespace Stock.API.Consumers
{
    public class StockRollBackMessageConsumer(MongoDbService _mongoDbService) : IConsumer<StockRollbackMessage>
    {
        public async Task Consume(ConsumeContext<StockRollbackMessage> context)
        {
            IMongoCollection<Models.Stock> stockCollection = _mongoDbService.GetCollection<Models.Stock>();


            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await (await stockCollection.FindAsync(x => x.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                stock.Count += orderItem.Count;
                await stockCollection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId, stock);

            }

        }
    }
}
