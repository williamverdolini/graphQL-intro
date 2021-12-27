using HotChocolate;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Publishers
{
    public class PublisherType : ObjectType<Publisher>
    {
        protected override void Configure(IObjectTypeDescriptor<Publisher> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(f => f.Id)
                .ResolveNodeWith<PublisherResolver>(r => r.ResolveAsync(default!, default!));
        }
    }

    public class PublisherResolver
    {
        public Task<Publisher> ResolveAsync(
            [Service] IMongoCollection<Publisher> collection,
            string id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}