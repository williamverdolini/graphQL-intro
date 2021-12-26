
using graphqlServer.Support;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddLogging()
    .AddSingleton<MongoContext>()
    .AddHostedService<DataSeeder>()
    .AddGraphQLServer()
    ;

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});

app.Run();
