using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace graphqlWarehouse.Support
{
    public class MongoContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoContext(IConfiguration configuration)
        {
            var traceEnabled = configuration.GetValue("TraceDBEnabled", false);
            var settings = new MongoClientSettings
            {
                ClusterConfigurator = cb =>
                {
                    if (traceEnabled)
                    {
                        // This will print the executed command to the console
                        cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        Console.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
                    });
                    }
                }
            };
            _client = new MongoClient(settings);
            _database = _client.GetDatabase("warehouse");
        }

        public IMongoClient Client => _client;

        public IMongoDatabase Database => _database;
    }
}