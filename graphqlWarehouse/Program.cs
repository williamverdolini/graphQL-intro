using graphqlWarehouse.Schema.Inventories;
using graphqlWarehouse.Support;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddLogging()
    .AddSingleton<MongoContext>()
    .AddHostedService<DataSeeder>()
    .AddScoped(sp => sp.GetRequiredService<MongoContext>().Database.GetCollection<Inventory>("inventory"))
    // GraphQL Schema Definition
    .AddGraphQLServer()
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<InventoryQueries>()
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
    .AddType<InventoryType>();    
    ;

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});
app.Run();
