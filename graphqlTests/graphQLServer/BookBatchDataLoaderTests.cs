using NUnit.Framework;
using System.Threading.Tasks;
using graphqlServer.Schema.Books;
using MongoDB.Driver;
using graphqlTests.Utils;

namespace graphqlTests.graphQLServer
{
    [TestFixture]
    public class BookBatchDataLoaderTests : MongoIntegrationTest
    {
        [Test]
        public async Task Loader_simple_test()
        {
            var books = CreateBookCollection(
                new Book{ Id = "75f31668-f348-4aa0-88c9-d186e0a6fe4e", Title = "Reconstituirea (Reconstruction)"}
            );
            var (loader, scheduler) = GetDataLoaderUT(books);

            var bookResult = loader.LoadAsync("75f31668-f348-4aa0-88c9-d186e0a6fe4e");
            scheduler.Dispatch();
            var book = await bookResult;

            Assert.IsNotNull(book);
            Assert.That(book.Id, Is.EqualTo("75f31668-f348-4aa0-88c9-d186e0a6fe4e"));
            Assert.That(book.Title, Is.EqualTo("Reconstituirea (Reconstruction)"));
        }

        [Test]
        public async Task Loader_multiple_keys_test()
        {
            var books = CreateBookCollection(
                new Book{ Id = "75f31668-f348-4aa0-88c9-d186e0a6fe4e", Title = "Reconstituirea (Reconstruction)"},
                new Book{ Id = "c8cf0474-5282-4286-aff5-f67c81b35efa", Title = "Clean Code"},
                new Book{ Id = "b96cfe8a-4b87-49f8-af61-38a17609cd3b", Title = "The Culture Code)"}
            );
            var (loader, scheduler) = GetDataLoaderUT(books);

            var bookResult = loader.LoadAsync(new string[]{
                "75f31668-f348-4aa0-88c9-d186e0a6fe4e",
                "c8cf0474-5282-4286-aff5-f67c81b35efa",
                "b96cfe8a-4b87-49f8-af61-38a17609cd3b"
            });
            scheduler.Dispatch();
            var book = await bookResult;

            Assert.IsNotNull(book);
            Assert.IsNotEmpty(book);
            Assert.That(book.Count, Is.EqualTo(3));
        }

        private (BookBatchDataLoader DataLoader, ManualBatchScheduler Scheduler) GetDataLoaderUT(IMongoCollection<Book> books)
        {
            var batchScheduler = new ManualBatchScheduler();
            return (new BookBatchDataLoader(books, batchScheduler), batchScheduler);
        }

        private IMongoCollection<Book> CreateBookCollection(params Book[] books) {
            var db = CreateDatabase("books-integration-tests");
            var collection = db.GetCollection<Book>("books");

            collection.InsertMany(books);
            return collection;
        }
    }
}