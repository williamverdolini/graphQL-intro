using HotChocolate.Types;

namespace graphqlServer.Schema.Publishers
{
    public class PublisherType : ObjectType<Publisher>
    {
        protected override void Configure(IObjectTypeDescriptor<Publisher> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(f => f.Id)
                .ResolveNode((ctx, id) => 
                    ctx.DataLoader<PublisherBatchDataLoader>().LoadAsync(id, ctx.RequestAborted));
        }
    }
}