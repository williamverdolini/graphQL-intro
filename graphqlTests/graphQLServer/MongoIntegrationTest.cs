using MongoDB.Driver;
using Mongo2Go;

namespace graphqlTests.graphQLServer
{
    public class MongoIntegrationTest
    {
        protected static MongoDbRunner _runner = default!;

        protected static IMongoDatabase CreateDatabase(string databaseName)
        {
            _runner = MongoDbRunner.Start();

            MongoClient client = new MongoClient(_runner.ConnectionString);
            return client.GetDatabase(databaseName);
        }
    }
}