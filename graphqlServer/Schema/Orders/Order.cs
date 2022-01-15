namespace graphqlServer.Schema.Orders
{
    public class Order
    {
        public string Id { get; set; } = default!;
        public string? BookId { get; set; }
        public string? UserName { get; set; }
        public DateTime BoughtOn { get; set; }
        public int Quantity { get; set; }
        public long Amount { get; set; }
    }
}