using HotChocolate;
using HotChocolate.Types;

namespace graphqlServer.Schema.Orders
{
    [ExtendObjectType("Subscription")]
    public class OrderSubscriptions
    {
        [Subscribe]
        public Order OrderConfirmed([EventMessage] Order order) => order;
    }
}