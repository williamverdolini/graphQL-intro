namespace graphqlWarehouse.Schema.Inventories
{
    public class Inventory
    {
        public string Id { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public ProductType ProductType { get; set; } = default!;
        public bool InStock { get; set; }
        public int AvailableQty { get; set; }
        public string? Plant { get; set; }
    }

    public enum ProductType
    {
        Unknown,
        Book,
        Stationery
    }
}