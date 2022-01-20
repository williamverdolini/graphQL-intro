using graphqlServer.Schema.Books;
using graphqlServer.Support;
using HotChocolate.Types;

namespace graphqlServer.Schema.Orders
{
    public class OrderType : ObjectType<Order>
    {
        protected override void Configure(IObjectTypeDescriptor<Order> descriptor)
        {
            descriptor
                .Field(f => f.BookId)
                .Name("book")
                .UseScalarProjection() // custom extentsion method
                .Type<BookType>()
                .Resolve((ctx, ct)
                    => ctx.DataLoader<BookBatchDataLoader>()
                            .LoadAsync(ctx.Parent<Order>().BookId!, ct));
        }
    }
}