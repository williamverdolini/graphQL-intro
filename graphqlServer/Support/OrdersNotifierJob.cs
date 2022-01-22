using graphqlServer.Schema.Orders;
using HotChocolate.Subscriptions;
using MongoDB.Driver;

namespace graphqlServer.Support
{
    public class OrdersNotifierJob : BackgroundService
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Order> _orders;
        private readonly ILogger<OrdersNotifierJob> _logger;
        private readonly ITopicEventSender _sender;

        public OrdersNotifierJob(
            ILogger<OrdersNotifierJob> logger,
            MongoContext ctx,
            ITopicEventSender sender
        )
        {
            _db = ctx.Database;
            _orders = _db.GetCollection<Order>("order");
            _logger = logger;
            _sender = sender;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(OrdersNotifierJob)} Executing");
            var cts = new CancellationTokenSource();
            return RT(() => DoWork(), seconds: 5, cts.Token);
        }

        private async Task DoWork()
        {
            var count = await _orders.CountDocumentsAsync(FilterDefinition<Order>.Empty).ConfigureAwait(false);
            if (count == 0)
            {
                return;
            }
            var rndOrder = new Random().Next(0, (int)count);

            var confirmedBook = _orders.AsQueryable().Skip(rndOrder).Take(1).FirstOrDefault();
            if (confirmedBook != null)
            {
                _logger.LogInformation($"Confirmed order id: {confirmedBook.Id}");
                await _sender.SendAsync(nameof(OrderSubscriptions.OrderConfirmed), confirmedBook);
            }
        }

        static Task RT(Func<Task> action, int seconds, CancellationToken token)
        {
            if (action == null)
                return Task.CompletedTask;
            return Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await action().ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(seconds), token);
                }
            }, token);
        }
    }
}