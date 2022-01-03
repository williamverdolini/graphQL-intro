using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using MongoDB.Driver;

namespace graphqlWarehouse.Schema.Inventories
{
    [ExtendObjectType("Query")]
    public class InventoryQueries
    {
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IExecutable<Inventory> GetInventories(
            [Service] IMongoCollection<Inventory> collection)
            => collection.AsExecutable();

        [UseFirstOrDefault]
        public Task<Inventory> GetInventoryById(
            InventoryBatchDataLoader loader,
            string id,
            CancellationToken ct)
            => loader.LoadAsync(id, ct);

        public async Task<Inventory[]> GetInventoriesByProductId(
                [ID] string productId,
                InventoryByProductDataLoader dataLoader)
                => await dataLoader.LoadAsync(productId);
    }
}