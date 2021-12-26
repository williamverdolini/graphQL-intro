using MongoDB.Driver;

namespace graphqlServer.Support
{
    public class MongoContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoContext()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("books");
        }

        public IMongoClient Client => _client;

        public IMongoDatabase Database => _database;
    }
}