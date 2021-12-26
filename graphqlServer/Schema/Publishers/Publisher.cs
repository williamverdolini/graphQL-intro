namespace graphqlServer.Schema.Publishers
{
    public class Publisher
    {
        public string Id { get; set; } = default!;
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}