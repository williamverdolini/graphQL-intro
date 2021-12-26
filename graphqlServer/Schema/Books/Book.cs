namespace graphqlServer.Schema.Books
{
    public class Book
    {
        public string Id { get; set; } = default!;
        public string? Title { get; set; }
        public string? Abstract { get; set; }
        public int? EditionVersion { get; set; }        
        public DateTime? PublicationDate { get; set; }
        public string[]? Categories { get; set; }
        public string[]? Authors { get; set; }
        public string? Publisher { get; set; }
        public string[]? RelatedBooks { get; set; }
    }
}