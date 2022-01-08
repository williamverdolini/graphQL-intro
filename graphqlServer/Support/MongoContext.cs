using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace graphqlServer.Support
{
    public class MongoContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoContext(IConfiguration configuration)
        {
            var traceEnabled = configuration.GetValue<bool>("TraceDBEnabled", false);
            
            var url = new MongoUrl(configuration.GetConnectionString("mongodb"));
            var settings = MongoClientSettings.FromUrl(url);
            settings.ClusterConfigurator = cb =>
                {
                    if (traceEnabled)
                    {
                        // This will print the executed command to the console
                        cb.Subscribe<CommandStartedEvent>(e =>
                        {
                            Console.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
                        });
                    }
                };            
            _client = new MongoClient(settings);
            _database = _client.GetDatabase(url.DatabaseName);
        }

        public IMongoClient Client => _client;

        public IMongoDatabase Database => _database;
    }
}