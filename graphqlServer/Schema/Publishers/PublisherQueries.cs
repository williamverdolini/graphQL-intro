using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Publishers
{
    [ExtendObjectType("Query")]
    public class PublisherQueries
    {
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IExecutable<Publisher> GetPublishers(
            [Service] IMongoCollection<Publisher> collection)
            => collection.AsExecutable();    

        [UseFirstOrDefault]
        public IExecutable<Publisher> GetPublisherById(
            [Service] IMongoCollection<Publisher> collection,
            string id)
        {
            return collection.Find(x => x.Id == id).AsExecutable();
        }   
    }
}