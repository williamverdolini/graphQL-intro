using GreenDonut;
using HotChocolate;
using MongoDB.Driver;

namespace graphqlServer.Schema.Publishers
{
    public class PublisherBatchDataLoader : BatchDataLoader<string, Publisher>
    {
        private readonly IMongoCollection<Publisher> _collection;

        public PublisherBatchDataLoader(
            [Service] IMongoCollection<Publisher> collection,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            _collection = collection;
        }

        protected override async Task<IReadOnlyDictionary<string, Publisher>> LoadBatchAsync(
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