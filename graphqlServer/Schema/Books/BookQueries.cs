using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Books
{
    [ExtendObjectType("Query")]
    public class BookQueries
    {
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IExecutable<Book> GetBooks(
            [Service] IMongoCollection<Book> collection)
            => collection.AsExecutable();

        [UseFirstOrDefault]
        public Task<Book> GetBookById(
            BookBatchDataLoader loader,
            string id,
            CancellationToken ct)
        {
            return loader.LoadAsync(id, ct);
        }
    }
}