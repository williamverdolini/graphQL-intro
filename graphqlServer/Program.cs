
using graphqlServer.Schema.Authors;
using graphqlServer.Schema.Books;
using graphqlServer.Schema.Middleware;
using graphqlServer.Schema.Publishers;
using graphqlServer.Support;
using HotChocolate.Execution.Instrumentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddLogging()
    // Monitoring
    .AddApplicationInsightsTelemetry()
    .AddSingleton<IExecutionDiagnosticEventListener, ApplicationInsightsDiagnosticEventListener>()
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
    // Global Object Identification
    .AddGlobalObjectIdentification()
    .AddType<AuthorType>()
    .AddType<PublisherType>()
    .AddType<BookType>()
    // Middlewares
    .AddDirectiveType<DecodeBase64DirectiveType>()
    ;

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});

app.Run();
