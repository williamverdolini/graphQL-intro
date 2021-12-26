using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Authors
{
    [ExtendObjectType("Query")]
    public class AuthorQueries
    {
        [UsePaging(IncludeTotalCount = true)]
        [UseProjection]
        [UseSorting]
        [UseFiltering]
        public IExecutable<Author> GetAuthors(
            [Service] IMongoCollection<Author> collection)
            => collection.AsExecutable();    

        [UseFirstOrDefault]
        public IExecutable<Author> GetAuthorById(
            [Service] IMongoCollection<Author> collection,
            string id)
        {
            return collection.Find(x => x.Id == id).AsExecutable();
        }
    }
}