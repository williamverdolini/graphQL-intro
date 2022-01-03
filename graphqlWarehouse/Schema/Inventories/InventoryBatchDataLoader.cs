using HotChocolate;
using MongoDB.Driver;
using GreenDonut;

namespace graphqlWarehouse.Schema.Inventories
{
    public class InventoryBatchDataLoader : BatchDataLoader<string, Inventory>
    {
        private readonly IMongoCollection<Inventory> _collection;

        public InventoryBatchDataLoader(
            [Service] IMongoCollection<Inventory> collection,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            _collection = collection;
        }

        protected override async Task<IReadOnlyDictionary<string, Inventory>> LoadBatchAsync(
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

        public async Task<IReadOnlyDictionary<string, Inventory>> LoadByProductIdsAsync(
            IReadOnlyList<string> keys,
            CancellationToken ct)
        {
            // instead of fetching one, we fetch multiple items
            var items = await _collection
                            .Find(x => keys.Contains(x.ProductId))
                            .Project(x => x.Id)
                            .ToListAsync(ct)
                            .ConfigureAwait(false);
            return await LoadBatchAsync(items as IReadOnlyList<string>, ct).ConfigureAwait(false);
        }
    }
}