using graphqlServer.Schema.Authors;
using graphqlServer.Schema.Publishers;
using HotChocolate;
using HotChocolate.Types;

namespace graphqlServer.Schema.Books
{
    public class BookType : ObjectType<Book>
    {
        protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(f => f.Id)
                .ResolveNodeWith<BookResolvers>(r => r.ResolveAsync(default!, default!, default));

            // Rewrite "authors" resolver to return complete book's authors
            descriptor
                .Field(f => f.Authors)
                .ResolveWith<BookResolvers>(t => t.GetAuthorsAsync(default!, default!, default!, default))
                .Authorize(roles: new [] {"admin"})
                ;

            // Rewrite "publisher" resolver to return complete book's publisher
            descriptor
                .Field(f => f.Publisher)
                .ResolveWith<BookResolvers>(t => t.GetPublisherAsync(default!, default!, default!, default))
                .Authorize(policy: "publishers.read");

            // Rewrite "relatedBooks" resolver to return complete book's related book info
            descriptor
                .Field(f => f.RelatedBooks)
                .ResolveWith<BookResolvers>(t => t.GetRelatedBooksAsync(default!, default!, default));
        }
    }

    public class BookResolvers
    {
        public Task<Book> ResolveAsync(
            BookBatchDataLoader loader,
            string id,
            CancellationToken ct)
        {
            return loader.LoadAsync(id, ct);
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync(
            [Parent] Book parent,
            BookBatchDataLoader bookLoader,
            AuthorBatchDataLoader authorLoader,
            CancellationToken ct)
        {
            var p = await bookLoader
                        .LoadAsync(parent.Id, ct)
                        .ConfigureAwait(false);
            if (p.Authors == null || p.Authors.Length == 0)
            {
                return new List<Author>();
            }
            return await authorLoader.LoadAsync(p.Authors, ct);
        }

        public async Task<Publisher?> GetPublisherAsync(
            [Parent] Book parent,
            BookBatchDataLoader bookLoader,
            PublisherBatchDataLoader publisherLoader,
            CancellationToken ct)
        {
            var p = await bookLoader.LoadAsync(parent.Id, ct).ConfigureAwait(false);
            if (p.Publisher == null)
            {
                return null;
            }
            return await publisherLoader.LoadAsync(p.Publisher, ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Book>> GetRelatedBooksAsync(
            [Parent] Book parent,
            BookBatchDataLoader bookLoader,
            CancellationToken ct)
        {
            var p = await bookLoader.LoadAsync(parent.Id, ct).ConfigureAwait(false);
            if (p.RelatedBooks == null || p.RelatedBooks.Length == 0)
            {
                return new List<Book>();
            }
            return await bookLoader.LoadAsync(p.RelatedBooks, ct).ConfigureAwait(false);
        }
    }
}