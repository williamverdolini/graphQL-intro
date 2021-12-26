using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Authors
{
    [ExtendObjectType("Query")]
    public class AuthorQueries
    {
        public IQueryable<Author> GetAuthors(
            [Service] IMongoCollection<Author> collection,
            int skip = 0,
            int limit = 50)
            => collection.AsQueryable().Skip(skip).Take(limit);   

        public Author GetAuthorById(
            [Service] IMongoCollection<Author> collection,
            string id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefault();
        }
    }
}