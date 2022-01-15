using graphqlServer.Schema.Books;
using HotChocolate.Types;

namespace graphqlServer.Schema.Orders
{
    public class OrderType : ObjectType<Order>
    {
        protected override void Configure(IObjectTypeDescriptor<Order> descriptor)
        {
            descriptor
                .Field(f => f.BookId)
                .IsProjected();

            descriptor
                .Field("book")
                .Resolve<Book>((ctx, ct) 
                    => ctx.DataLoader<BookBatchDataLoader>()
                            .LoadAsync(ctx.Parent<Order>().BookId!, ct));
        }
    }
}