using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Authors
{
    [ExtendObjectType("Query")]
    public class AuthorQueries
    {
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IExecutable<Author> GetAuthors(
            [Service] IMongoCollection<Author> collection)
            => collection.AsExecutable();

        [UseFirstOrDefault]
        public Task<Author> GetAuthorById(
            AuthorBatchDataLoader loader,
            string id,
            CancellationToken ct)
        {
            return loader.LoadAsync(id, ct);
        }
    }
}