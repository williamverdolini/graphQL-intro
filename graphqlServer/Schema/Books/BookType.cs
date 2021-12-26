using graphqlServer.Schema.Authors;
using graphqlServer.Schema.Publishers;
using HotChocolate;
using HotChocolate.Types;
using MongoDB.Driver;

namespace graphqlServer.Schema.Books
{
    public class BookType : ObjectType<Book>
    {
        protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(f => f.Id)
                .ResolveNodeWith<BookResolvers>(r => r.ResolveAsync(default!, default!));

            // Rewrite "authors" resolver to return complete book's authors
            descriptor
                .Field(f => f.Authors)
                .ResolveWith<BookResolvers>(t => t.GetAuthorsAsync(default!, default!, default!, default));

            // Rewrite "publisher" resolver to return complete book's publisher
            descriptor
                .Field(f => f.Publisher)
                .ResolveWith<BookResolvers>(t => t.GetPublisherAsync(default!, default!, default!, default));

            // Rewrite "relatedBooks" resolver to return complete book's related book info
            descriptor
                .Field(f => f.RelatedBooks)
                .ResolveWith<BookResolvers>(t => t.GetRelatedBooksAsync(default!, default!, default));
        }
    }

    public class BookResolvers
    {
        public Task<Book> ResolveAsync(
            [Service] IMongoCollection<Book> collection,
            string id)
        {
            return collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync(
            [Parent] Book parent,
            [Service] IMongoCollection<Book> books,
            [Service] IMongoCollection<Author> authors,
            CancellationToken cancellationToken)
        {
            var authorIds = await books
                        .Find(b => b.Id == parent.Id)
                        .Project(b => b.Authors)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
            if(authorIds?.Length == 0) {
                return new List<Author>();
            }
            return await authors
                .Find(a => authorIds!.Contains(a.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<Publisher?> GetPublisherAsync(
            [Parent] Book parent,
            [Service] IMongoCollection<Book> books,
            [Service] IMongoCollection<Publisher> publishers,
            CancellationToken cancellationToken)
        {
            var publisherId = await books
                        .Find(b => b.Id == parent.Id && b.Publisher != null)
                        .Project(b => b.Publisher)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
            if(publisherId == null) {
                return null;
            }
            return await publishers
                .Find(a => a.Id == publisherId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Book>> GetRelatedBooksAsync(
            [Parent] Book parent,
            [Service] IMongoCollection<Book> books,
            CancellationToken cancellationToken)
        {
            var relatedIds = await books
                        .Find(b => b.Id == parent.Id && b.RelatedBooks != null)
                        .Project(b => b.RelatedBooks)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
            if(relatedIds?.Length == 0) {
                return new List<Book>();
            }
            return await books
                .Find(a => relatedIds!.Contains(a.Id))
                .ToListAsync(cancellationToken);
        }
    }

}