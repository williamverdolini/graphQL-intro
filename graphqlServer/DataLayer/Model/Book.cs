using MongoDB.Bson.Serialization.Attributes;

namespace graphqlServer.DataLayer.Model
{
    public class Book
    {
        [BsonId]
        public string Id { get; set; } = default!;
        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string[]? Categories { get; set; }
        public string[]? Authors { get; set; }
        public string? Publisher { get; set; }
        public string[]? RelatedBooks { get; set; }
    }

    public class Author
    {

        [BsonId]
        public string Id { get; set; } = default!;
        public string? FirstName { get; set; }
        public string? SurnName { get; set; }
        public string? WebSite { get; set; }
        public string? Email { get; set; }
    }

    public class Publisher
    {

        [BsonId]
        public string Id { get; set; } = default!;
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}