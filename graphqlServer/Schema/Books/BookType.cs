using HotChocolate;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Books
{
    public class BookType: ObjectType<Book>
    {
        protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(f => f.Id)
                .ResolveNodeWith<BookResolver>(r => r.ResolveAsync(default!, default!));
        }
    }

    public class BookResolver
    {
        public Task<Book> ResolveAsync(
            [Service] IMongoCollection<Book> collection,
            string id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }

}