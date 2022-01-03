using HotChocolate.Types;

namespace graphqlWarehouse.Schema.Inventories
{
    public class InventoryType : ObjectType<Inventory>
    {
        // for Schema stitching we HAVE to remove GOI: https://github.com/ChilliCream/hotchocolate/issues/3913#issuecomment-872554639
        protected override void Configure(IObjectTypeDescriptor<Inventory> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(f => f.Id)
                .ResolveNode((ctx, id) =>
                    ctx.DataLoader<InventoryBatchDataLoader>().LoadAsync(id, ctx.RequestAborted));
        }
    }
}