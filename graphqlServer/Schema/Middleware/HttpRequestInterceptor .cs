using System.Security.Claims;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;

namespace graphqlServer.Schema.Middleware
{
    public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
    {
        public override ValueTask OnCreateAsync(HttpContext context,
            IRequestExecutor requestExecutor, IQueryRequestBuilder requestBuilder,
            CancellationToken cancellationToken)
        {
            string? userName = context.User.FindFirst(ClaimTypes.Name)?.Value;
            requestBuilder.SetProperty("UserName", userName);

            return base.OnCreateAsync(context, requestExecutor, requestBuilder,
                cancellationToken);
        }
    }

    public class UserNameAttribute : GlobalStateAttribute
    {
        public UserNameAttribute() : base("UserName")
        {
        }
    }

}