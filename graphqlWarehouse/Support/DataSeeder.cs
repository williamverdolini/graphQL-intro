using graphqlWarehouse.DataLayer.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace graphqlWarehouse.Support
{
    public class DataSeeder : BackgroundService
    {
        private string DATA => Path.Combine(Directory.GetCurrentDirectory(), "DataLayer", "Data");
        private string Inventory_DATA => Path.Combine(DATA, "Inventory_DATA.json");
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Inventory> _inventories;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(
            MongoContext ctx,
            ILogger<DataSeeder> logger)
        {
            _db = ctx.Database;
            _inventories = _db.GetCollection<Inventory>("inventory");
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var isInitialized = await IsDbInitializedAsync().ConfigureAwait(false);
                if (isInitialized)
                {
                    _logger.LogInformation("Mongo Db \"warehouse\" is initialized");
                    return;
                }
                _logger.LogInformation("Mongo Db initialization starts...");
                var InventoryIds = await BulkInsertInventorysAsync().ConfigureAwait(false);
                _logger.LogInformation("Mongo Db initialization ends...");
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

        private async Task<List<string>> BulkInsertInventorysAsync()
        {
            return await BulkInsertAsync(_inventories, Inventory_DATA,
                d => new Inventory
                {
                    Id = d["id"].AsString,
                    ProductId = d["productId"].AsString,
                    ProductType = d["productType"].AsString,
                    InStock = d["inStock"].AsBoolean,
                    AvailableQty = d["availableQty"].AsInt32,
                    Plant = d["plant"].IsBsonNull ? null : d["plant"].AsString,
                }).ConfigureAwait(false);
        }
        private Task<List<string>> BulkInsertAsync<T>(
            IMongoCollection<T> collection,
            string dataFilePath,
            Func<BsonDocument, T> mapOperation)
        {
            return BulkInsertAsync(collection, GetDocuments(dataFilePath), mapOperation);
        }

        private async Task<List<string>> BulkInsertAsync<T>(
            IMongoCollection<T> collection,
            IEnumerable<BsonDocument> documents,
            Func<BsonDocument, T> mapOperation)
        {
            var ids = new List<string>();
            var bulk = new List<WriteModel<T>>();
            var operations = documents
                        .Select(d =>
                        {
                            var id = d["id"].AsString;
                            var upsertOne = new ReplaceOneModel<T>(
                                Builders<T>.Filter.And(Builders<T>.Filter.Eq("Id", id)),
                                mapOperation(d));
                            upsertOne.IsUpsert = true;
                            ids.Add(id);
                            return upsertOne;
                        })
                        .ToList();
            bulk.AddRange(operations);

            await collection
                .BulkWriteAsync(bulk, new BulkWriteOptions { IsOrdered = false })
                .ConfigureAwait(false);

            return ids;
        }

        private IEnumerable<BsonDocument> GetDocuments(string dataFilePath)
        {
            string json = File.ReadAllText(dataFilePath);
            return BsonSerializer
                        .Deserialize<BsonArray>(json)
                        .Select(d => d.AsBsonDocument);
        }

        private List<string> GetIds(IEnumerable<BsonDocument> documents)
        {
            return documents.Select(d => d["id"].AsString).ToList();
        }
        private async Task<bool> IsDbInitializedAsync()
        {
            var dbs = await _db.Client
                            .ListDatabases()
                            .ToListAsync()
                            .ConfigureAwait(false);
            return dbs.Any(d => d.Contains("name") && d["name"] == _db.DatabaseNamespace.DatabaseName);
        }
    }
}