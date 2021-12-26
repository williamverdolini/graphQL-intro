using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Books
{
    [ExtendObjectType("Query")]
    public class BookQueries
    {
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IExecutable<Book> GetBooks(
            [Service] IMongoCollection<Book> collection)
            => collection.AsExecutable();    

        [UseFirstOrDefault]
        public IExecutable<Book> GetBookById(
            [Service] IMongoCollection<Book> collection,
            string id)
        {
            return collection.Find(x => x.Id == id).AsExecutable();
        }
    }
}