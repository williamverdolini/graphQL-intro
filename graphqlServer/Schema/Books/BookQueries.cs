using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Books
{
    [ExtendObjectType("Query")]
    public class BookQueries
    {
        public IQueryable<Book> GetBooks(
            [Service] IMongoCollection<Book> collection,
            int skip = 0,
            int limit = 50)
            => collection.AsQueryable().Skip(skip).Take(limit);    

        public Book GetBookById(
            [Service] IMongoCollection<Book> collection,
            string id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }
    }
}