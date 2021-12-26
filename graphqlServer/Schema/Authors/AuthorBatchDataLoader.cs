using HotChocolate;
using MongoDB.Driver;
using GreenDonut;

namespace graphqlServer.Schema.Authors
{
    public class AuthorBatchDataLoader : BatchDataLoader<string, Author>
    {
        private readonly IMongoCollection<Author> _collection;

        public AuthorBatchDataLoader(
            [Service] IMongoCollection<Author> collection,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            _collection = collection;
        }

        protected override async Task<IReadOnlyDictionary<string, Author>> LoadBatchAsync(
            IReadOnlyList<string> keys,
            CancellationToken ct)
        {
            // instead of fetching one, we fetch multiple items
            var items = await _collection
                            .Find(x => keys.Contains(x.Id))
                            .ToListAsync(ct)
                            .ConfigureAwait(false);
            return items.ToDictionary(x => x.Id);
        }
    }
}