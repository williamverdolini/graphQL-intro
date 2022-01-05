using HotChocolate.Types;

namespace graphqlWarehouse.Schema.Inventories
{
    public class InventoryType : ObjectType<Inventory>
    {
        protected override void Configure(IObjectTypeDescriptor<Inventory> descriptor)
        {           
            descriptor
                .ImplementsNodeWithDbIdField(
                    idProperty: f => f.Id, 
                    nodeResolver: (ctx, id) =>
                        ctx.DataLoader<InventoryBatchDataLoader>().LoadAsync(id, ctx.RequestAborted));
        }
    }
}