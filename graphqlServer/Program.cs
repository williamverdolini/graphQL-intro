using graphqlServer.Schema.Authors;
using graphqlServer.Schema.Books;
using graphqlServer.Schema.Middleware;
using graphqlServer.Schema.Orders;
using graphqlServer.Schema.Publishers;
using graphqlServer.Support;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Language;
using HotChocolate.Data.Projections;

var builder = WebApplication.CreateBuilder(args);

// Authentication/Authorization
builder.Services
    .AddJwtAuthentication(builder.Configuration);

builder.Services
    // Custom authorization policies
    .AddAuthorizationPolicies()
    .AddLogging()
    .AddMemoryCache()
    .AddSha256DocumentHashProvider(HashFormat.Hex)
    // Monitoring
    .AddApplicationInsightsTelemetry()
    .AddSingleton<IExecutionDiagnosticEventListener, ApplicationInsightsDiagnosticEventListener>()
    .AddSingleton<MongoContext>()
    .AddHostedService<DataSeeder>()
    .AddHostedService<OrdersDataSeeder>()
    .AddHostedService<OrdersNotifierJob>()
    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Author>("author"))
    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Publisher>("publisher"))
    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Book>("book"))
    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Order>("order"))
    // GraphQL Schema Definition
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<AuthorQueries>()
        .AddTypeExtension<PublisherQueries>()
        .AddTypeExtension<BookQueries>()
        .AddTypeExtension<UserOrderQueries>()
    .AddInMemorySubscriptions()
    .AddSubscriptionType(d => d.Name("Subscription"))
        .AddTypeExtension<OrderSubscriptions>()
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
    .AddType<OrderType>()
    // Middlewares
    .AddDirectiveType<DecodeBase64DirectiveType>()
    .AddHttpRequestInterceptor<HttpRequestInterceptor>()
    .AddConvention<IProjectionConvention>(
                new ProjectionConventionExtension(
                    x => x.AddProviderExtension(
                        new ProjectionProviderExtension(y => y.RegisterFieldHandler<ConditionFieldProjectionHandler>()))))
    // Automatic Persisted Queries
    .UseAutomaticPersistedQueryPipeline()
    .AddInMemoryQueryStorage()
    // Security
    .SetSecurity()
    // Schema Federation
    .InitializeOnStartup()
    .PublishSchemaDefinition(c => c
        // The name of the schema. This name should be unique
        .SetName("books")
        .AddTypeExtensionsFromFile("./Schema/Stitching.graphql"))
    ;

builder.Services.AddCors(options =>
{
    options.AddPolicy("ng-client",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            ;
                      });
});

var app = builder.Build();

app.UseCors("ng-client");
app.MapGet("/", () => "Hello World!");
app.UseRouting();
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
    endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Index}/{id?}");
});
app.UseGraphQLSchemaPrint(Path.Combine(Directory.GetCurrentDirectory(), "..", "artifacts", "schema"));

app.Run();
