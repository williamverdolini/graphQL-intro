using HotChocolate;
using MongoDB.Driver;
using GreenDonut;

namespace graphqlWarehouse.Schema.Inventories
{
    public class InventoryByProductDataLoader : GroupedDataLoader<string, Inventory>
    {
        private readonly IMongoCollection<Inventory> _collection;

        public InventoryByProductDataLoader(
            [Service] IMongoCollection<Inventory> collection,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            _collection = collection;
        }

        protected override async Task<ILookup<string, Inventory>> LoadGroupedBatchAsync(
            IReadOnlyList<string> productIds,
            CancellationToken ct)
        {
            var items = await _collection
                            .Find(x => productIds.Contains(x.ProductId))
                            .ToListAsync(ct)
                            .ConfigureAwait(false);            
            return items.ToLookup(x => x.ProductId);
        }
    }
}