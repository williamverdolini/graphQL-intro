using graphqlIntegration;
using StrawberryShake.Extensions;

public class OrderConfirmedSubscriber
{
    private readonly IgraphQlServerClient _client;
    private IDisposable? _subscription;

    public OrderConfirmedSubscriber(IgraphQlServerClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public void Start()
        => _subscription = _client
            .OnOrderConfirmed
            .Watch()
            .Subscribe(r =>
            {
                var order = r.Data!.OrderConfirmed;
                Console.WriteLine($@"
        Congratulations {order.UserName}!
        Your order for ""{order.Book!.Title}"" has been successfully confirmed.
        Thank you and come back soon!");
            });

    public void Stop() => _subscription!.Dispose();
}