
using graphqlServer.Schema.Authors;
using graphqlServer.Schema.Books;
using graphqlServer.Schema.Publishers;
using graphqlServer.Support;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddLogging()
    .AddSingleton<MongoContext>()
    .AddHostedService<DataSeeder>()
    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Author>("author"))
    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Publisher>("publisher"))
    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Book>("book"))
    // GraphQL Schema Definition
    .AddGraphQLServer()
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<AuthorQueries>()
        .AddTypeExtension<PublisherQueries>()
        .AddTypeExtension<BookQueries>()
    // Registers the filter convention of MongoDB
    .AddMongoDbFiltering()
    // Registers the sorting convention of MongoDB
    .AddMongoDbSorting()
    // Registers the projection convention of MongoDB
    .AddMongoDbProjections()
    // Registers the paging providers of MongoDB
    .AddMongoDbPagingProviders()
    ;

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});

app.Run();
