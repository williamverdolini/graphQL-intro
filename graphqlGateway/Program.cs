var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("books", c => c.BaseAddress = new Uri("https://localhost:7059/graphql"));
builder.Services.AddHttpClient("inventories", c => c.BaseAddress = new Uri("https://localhost:7016/graphql"));

builder.Services
    .AddLogging()
    .AddGraphQLServer()
    .AddRemoteSchema("books")
    .AddRemoteSchema("inventories")
    ;


var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});

app.Run();
