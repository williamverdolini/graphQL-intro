using graphqlServer.DataLayer.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace graphqlServer.Support
{
    public class DataSeeder : BackgroundService
    {
        private string DATA => Path.Combine(Directory.GetCurrentDirectory(), "DataLayer", "Data");
        private string PUBLISHER_DATA => Path.Combine(DATA, "Publisher_DATA.json");
        private string AUTHOR_DATA => Path.Combine(DATA, "Author_DATA.json");
        private string BOOK_DATA => Path.Combine(DATA, "Book_DATA.json");
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Publisher> _publishers;
        private readonly IMongoCollection<Author> _authors;
        private readonly IMongoCollection<Book> _books;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(
            MongoContext ctx,
            ILogger<DataSeeder> logger)
        {
            _db = ctx.Database;
            _publishers = _db.GetCollection<Publisher>("publisher");
            _authors = _db.GetCollection<Author>("author");
            _books = _db.GetCollection<Book>("book");
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var isInitialized = await IsDbInitializedAsync().ConfigureAwait(false);
                if (isInitialized)
                {
                    _logger.LogInformation("Mongo Db \"books\" is initialized");
                    return;
                }
                _logger.LogInformation("Mongo Db initialization starts...");
                var publisherIds = await BulkInsertPublishersAsync().ConfigureAwait(false);
                var authorIds = await BulkInsertAuthorsAsync().ConfigureAwait(false);
                await BulkInsertBooksAsync(authorIds, publisherIds).ConfigureAwait(false);
                _logger.LogInformation("Mongo Db initialization starts...");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Mongo Db is NOT running...Please start the MongoDb server");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in data seeding!!");
                throw;
            }
        }

        private async Task<List<string>> BulkInsertAuthorsAsync()
        {
            return await BulkInsertAsync(_authors, AUTHOR_DATA,
                d => new Author
                {
                    Id = d["id"].AsString,
                    FirstName = d["firstName"].AsString,
                    SurnName = d["lastName"].AsString,
                    Email = d["email"].AsString,
                    WebSite = d["webSite"].AsString
                }).ConfigureAwait(false);
        }

        private async Task<List<string>> BulkInsertPublishersAsync()
        {
            return await BulkInsertAsync(_publishers, PUBLISHER_DATA,
                d => new Publisher
                {
                    Id = d["id"].AsString,
                    Name = d["name"].AsString,
                    Address = d["address"].AsString
                }).ConfigureAwait(false);
        }

        private async Task<List<string>> BulkInsertBooksAsync(
            List<string> authorIds,
            List<string> publisherIds)
        {
            var bookDocs = GetDocuments(BOOK_DATA);
            var bookIds = GetIds(bookDocs);

            return await BulkInsertAsync(_books, bookDocs,
                d => new Book
                {
                    Id = d["id"].AsString,
                    Title = d["title"].AsString,
                    Abstract = d["abstract"].AsString,
                    PublicationDate = DateTimeOffset.Parse(d["publicationDate"].AsString).UtcDateTime,
                    Categories = d["abstract"].AsString.Split("|").ToArray(),
                    Authors = authorIds
                            .Skip(new Random().Next(authorIds.Count - 3))
                            .Take(new Random().Next(1, 4)).ToArray(),
                    Publisher = publisherIds
                            .Skip(new Random().Next(authorIds.Count - 1))
                            .Take(1)
                            .FirstOrDefault(),
                    RelatedBooks = bookIds
                            .Where(b => b != d["id"].AsString)
                            .Skip(new Random().Next(authorIds.Count - 10))
                            .Take(new Random().Next(10)).ToArray()
                }).ConfigureAwait(false);
        }

        private Task<List<string>> BulkInsertAsync<T>(
            IMongoCollection<T> collection,
            string dataFilePath,
            Func<BsonDocument, T> mapOperation)
        {
            return BulkInsertAsync<T>(collection, GetDocuments(dataFilePath), mapOperation);
        }

        private async Task<List<string>> BulkInsertAsync<T>(
            IMongoCollection<T> collection,
            IEnumerable<BsonDocument> documents,
            Func<BsonDocument, T> mapOperation)
        {
            var ids = new List<string>();
            var bulk = new List<WriteModel<T>>();
            var operations = documents
                        .Select(d =>
                        {
                            var id = d["id"].AsString;
                            var upsertOne = new ReplaceOneModel<T>(
                                Builders<T>.Filter.And(Builders<T>.Filter.Eq("Id", id)),
                                mapOperation(d));
                            upsertOne.IsUpsert = true;
                            ids.Add(id);
                            return upsertOne;
                        })
                        .ToList();
            bulk.AddRange(operations);

            await collection
                .BulkWriteAsync(bulk, new BulkWriteOptions { IsOrdered = false })
                .ConfigureAwait(false);

            return ids;
        }

        private IEnumerable<BsonDocument> GetDocuments(string dataFilePath)
        {
            string json = File.ReadAllText(dataFilePath);
            return BsonSerializer
                        .Deserialize<BsonArray>(json)
                        .Select(d => d.AsBsonDocument);
        }

        private List<string> GetIds(IEnumerable<BsonDocument> documents)
        {
            return documents.Select(d => d["id"].AsString).ToList();
        }
        private async Task<bool> IsDbInitializedAsync()
        {
            var dbs = await _db.Client
                            .ListDatabases()
                            .ToListAsync()
                            .ConfigureAwait(false);
            return dbs.Any(d => d.Contains("name") && d["name"] == _db.DatabaseNamespace.DatabaseName);
        }
    }
}