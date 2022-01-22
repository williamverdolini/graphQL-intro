using graphqlServer.Schema.Middleware;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Orders
{
    [ExtendObjectType("Query")]
    public class UserOrderQueries
    {
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        [Authorize]
        public IExecutable<Order> GetUserOrders(
            [UserName] string userName,
            [Service] IMongoCollection<Order> collection)
            => collection.Find(Builders<Order>.Filter.Eq(o => o.UserName, userName)).AsExecutable();

        [UseFirstOrDefault]
        [Authorize]
        public Task<Order> GetUserOrderById(
            IResolverContext context,
            [Service] IMongoCollection<Order> collection,
            string id,
            CancellationToken ct)
        {
            // with inline dataloader, we can access context info 
            // Here Authorize guarantees that userName is populated (but is not necessary)
            return context.BatchDataLoader<string, Order>(async (keys, ct) =>
            {
                // instead of fetching one, we fetch multiple items
                var items = await collection
                        .Find(x => keys.Contains(x.Id)
                            && x.UserName == context.GetGlobalValue<string>("UserName"))
                        .ToListAsync(ct)
                        .ConfigureAwait(false);
                return items.ToDictionary(x => x.Id);
            })
            .LoadAsync(id);
        }
        
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IExecutable<Order> GetOrders(
            [Service] IMongoCollection<Order> collection)
            => collection.AsExecutable();

        [UseFirstOrDefault]
        public Task<Order> GetOrderById(
            IResolverContext context,
            [Service] IMongoCollection<Order> collection,
            string id,
            CancellationToken ct)
        {
            // with inline dataloader, we can access context info 
            // Here Authorize guarantees that userName is populated (but is not necessary)
            return context.BatchDataLoader<string, Order>(async (keys, ct) =>
            {
                // instead of fetching one, we fetch multiple items
                var items = await collection
                        .Find(x => keys.Contains(x.Id))
                        .ToListAsync(ct)
                        .ConfigureAwait(false);
                return items.ToDictionary(x => x.Id);
            })
            .LoadAsync(id);
        }
    }
}