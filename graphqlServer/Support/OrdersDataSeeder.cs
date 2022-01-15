using graphqlServer.Controllers.Auth;
using graphqlServer.DataLayer.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace graphqlServer.Support
{
    public class OrdersDataSeeder : BackgroundService
    {
        private string DATA => Path.Combine(Directory.GetCurrentDirectory(), "DataLayer", "Data");
        private string PUBLISHER_DATA => Path.Combine(DATA, "Order_DATA.json");
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<BookOrder> _orders;
        private readonly IMongoCollection<Book> _books;
        private readonly IUserRepository _users;
        private readonly ILogger<DataSeeder> _logger;

        public OrdersDataSeeder(
            MongoContext ctx,
            IUserRepository users,
            ILogger<DataSeeder> logger)
        {
            _db = ctx.Database;
            _orders = _db.GetCollection<BookOrder>("order");
            _books = _db.GetCollection<Book>("book");
            _users = users;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var count = await _orders.CountDocumentsAsync(FilterDefinition<BookOrder>.Empty).ConfigureAwait(false);
                var isInitialized = count > 0;
                if (isInitialized)
                {
                    _logger.LogInformation("Collection \"orders\" is initialized");
                    return;
                }
                _logger.LogInformation("Mongo Db Collection \"orders\" initialization starts...");
                await BulkInsertOrdersAsync().ConfigureAwait(false);
                _logger.LogInformation("Mongo Db Collection \"orders\" initialization finished");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Mongo Db is NOT running...Please start the MongoDb server");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in data seeding!!");
                throw;
            }
        }

        private bool IsCollectionInitializedAsync(string collectionName)
        {
            return _db.GetCollection<BsonDocument>(collectionName).CountDocuments(FilterDefinition<BsonDocument>.Empty) > 0;
        }

        private async Task BulkInsertOrdersAsync()
        {
            var booksId = _books.AsQueryable().Select(b => b.Id).ToList();
            var usernames = _users.GetUserNames();

            var bulk = new List<WriteModel<BookOrder>>();
            var operations = booksId
                        .Select(bId => new BookOrder
                        {
                            Id = Guid.NewGuid().ToString(),
                            BookId = bId,
                            UserName = usernames[new Random().Next(0, usernames.Length)],
                            Quantity = new Random().Next(1, 10),
                            Amount = new Random().Next(10, 100),
                            BoughtOn = new DateTime(new Random().Next(2000, 2022), new Random().Next(1, 12), new Random().Next(1, 28))
                        })
                        .Select(d =>
                        {
                            var upsertOne = new ReplaceOneModel<BookOrder>(
                                Builders<BookOrder>.Filter.And(Builders<BookOrder>.Filter.Eq("Id", d.Id)),
                                d);
                            upsertOne.IsUpsert = true;
                            return upsertOne;
                        })
                        .ToList();
            bulk.AddRange(operations);

            await _orders
                .BulkWriteAsync(bulk, new BulkWriteOptions { IsOrdered = false })
                .ConfigureAwait(false);

        }


        private IEnumerable<BsonDocument> GetDocuments(string dataFilePath)
        {
            string json = File.ReadAllText(dataFilePath);
            return BsonSerializer
                        .Deserialize<BsonArray>(json)
                        .Select(d => d.AsBsonDocument);
        }
    }
}