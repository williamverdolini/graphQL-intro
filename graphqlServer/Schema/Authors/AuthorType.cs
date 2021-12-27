using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Authors
{
    public class AuthorType : ObjectType<Author>
    {
        protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(f => f.Id)
                .ResolveNode(async (ctx, id) => {
                    var author = await ctx.Service<IMongoCollection<Author>>().Find(x => x.Id == id).FirstOrDefaultAsync();
                    return author;
                });
        }
    }
}