using HotChocolate.Types;

namespace graphqlServer.Schema.Authors
{
    public class AuthorType : ObjectType<Author>
    {
        protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(f => f.Id)
                .ResolveNode((ctx, id) => 
                    ctx.DataLoader<AuthorBatchDataLoader>().LoadAsync(id, ctx.RequestAborted));
        }
    }
}