using System.IO;
using System.Threading;
using System.Threading.Tasks;
using graphqlServer.Schema.Authors;
using graphqlServer.Schema.Books;
using graphqlServer.Schema.Publishers;
using graphqlServer.Support;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace graphqlTests.graphQLServer
{
    public abstract class GraphQLServerBaseIntegrationTest
    {
        protected IRequestExecutorBuilder _requestBuilder = default!;
        private ServiceProvider _serviceProvider = default!;
        private DataSeeder _seeder = default!;
        private MongoContext _dbCtx = default!;

        [OneTimeSetUp]
        public Task OnOneTimeSetupAsync()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection()
                    .AddLogging()
                    .AddSingleton<IConfiguration>(config)
                    .AddSingleton<MongoContext>()
                    .AddSingleton<DataSeeder>()
                    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Author>("author"))
                    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Publisher>("publisher"))
                    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Book>("book"))
                    ;

            _requestBuilder = SetupRequestBuilder(services);
            _serviceProvider = services.BuildServiceProvider();

            _seeder = _serviceProvider.GetRequiredService<DataSeeder>();
            _dbCtx = _serviceProvider.GetRequiredService<MongoContext>();

            return PopulateDatabaseAsync();
        }

        [OneTimeTearDown]
        public Task OnOneTimeTearDownAsync()
        {
            return DropDatabaseAsync();
        }
        protected Task PopulateDatabaseAsync()
        {
            return _seeder.StartAsync(CancellationToken.None);
        }

        protected Task DropDatabaseAsync()
        {
            return _dbCtx.Client.DropDatabaseAsync(_dbCtx.Database.DatabaseNamespace.DatabaseName);
        }

        protected ValueTask<ISchema> BuildSchema(IServiceCollection services = null!)
        {
            return SetupRequestBuilder(services)
                .BuildSchemaAsync();
        }

        protected IRequestExecutorBuilder SetupRequestBuilder(IServiceCollection services = null!)
        {
            return (services ?? new ServiceCollection())
                .AddGraphQL()
                .AddQueryType(c => c.Name("Query"))
                .AddTypeExtension<AuthorQueries>()
                .AddTypeExtension<PublisherQueries>()
                .AddTypeExtension<BookQueries>()
                .AddMongoDbFiltering()
                .AddMongoDbSorting()
                .AddMongoDbProjections()
                .AddMongoDbPagingProviders()
                .AddGlobalObjectIdentification()
                .AddType<AuthorType>()
                .AddType<PublisherType>()
                .AddType<BookType>();
        }
    }
}