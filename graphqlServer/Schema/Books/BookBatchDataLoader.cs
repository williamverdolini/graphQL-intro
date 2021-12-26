using GreenDonut;
using HotChocolate;
using MongoDB.Driver;

namespace graphqlServer.Schema.Books
{
    public class BookBatchDataLoader : BatchDataLoader<string, Book>
    {
        private readonly IMongoCollection<Book> _collection;

        public BookBatchDataLoader(
            [Service] IMongoCollection<Book> collection,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            _collection = collection;
        }

        protected override async Task<IReadOnlyDictionary<string, Book>> LoadBatchAsync(
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