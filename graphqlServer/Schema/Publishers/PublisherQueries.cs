using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Publishers
{
    [ExtendObjectType("Query")]
    public class PublisherQueries
    {
        public IQueryable<Publisher> GetPublishers(
            [Service] IMongoCollection<Publisher> collection,
            int skip = 0,
            int limit = 50)
            => collection.AsQueryable().Skip(skip).Take(limit);    

        public Publisher GetPublisherById(
            [Service] IMongoCollection<Publisher> collection,
            string id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }   
    }
}