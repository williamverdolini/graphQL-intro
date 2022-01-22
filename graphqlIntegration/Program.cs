using Microsoft.Extensions.DependencyInjection;
using graphqlIntegration;

Console.WriteLine("**************************************");
Console.WriteLine("***** GraphQL Integration client *****");
Console.WriteLine("**************************************");

var serviceCollection = new ServiceCollection();
serviceCollection
    .AddSingleton<OrderConfirmedSubscriber>()
    .AddgraphQlServerClient()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://localhost:7059/graphql"))
    .ConfigureWebSocketClient(client => client.Uri = new Uri("wss://localhost:7059/graphql"))
    ;

Console.WriteLine("");
Console.WriteLine("========> Press enter to read a book:");
Console.ReadLine();

IServiceProvider services = serviceCollection.BuildServiceProvider();
IgraphQlServerClient client = services.GetRequiredService<IgraphQlServerClient>();
var order = await client.OrderById.ExecuteAsync("bbedc085-68b0-4234-8c19-13c3b67db43d").ConfigureAwait(false);

Console.WriteLine($"Can read this book: {(order?.Data?.Orders?.Nodes?[0]?.Book?.Title ?? "no book")}");

Console.WriteLine("");
Console.WriteLine("========> Press enter to subscribe to new orders.");
Console.ReadLine();

var ordersConfirmed = services.GetRequiredService<OrderConfirmedSubscriber>();
ordersConfirmed.Start();

Console.WriteLine("Press enter to STOP.");
Console.ReadLine();

ordersConfirmed.Stop();
