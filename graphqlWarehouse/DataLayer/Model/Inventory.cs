using MongoDB.Bson.Serialization.Attributes;

namespace graphqlWarehouse.DataLayer.Model
{
    public class Inventory
    {
        [BsonId]
        public string Id { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public string ProductType { get; set; } = default!;
        public bool InStock { get; set; }
        public int AvailableQty { get; set; }
        public string? Plant { get; set; }
    }
}