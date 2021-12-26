using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;

namespace graphqlServer.Controllers.Auth
{
    public class CanReadPublishersRequirement : IAuthorizationRequirement { }

    public class CanReadPublishersAuthorizationHandler
        : AuthorizationHandler<CanReadPublishersRequirement, IResolverContext>
    {
        private readonly IUserRepository users;

        public CanReadPublishersAuthorizationHandler(IUserRepository users)
        {
            this.users = users;
        }
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanReadPublishersRequirement requirement,
            IResolverContext resource)
        {
            if (context.User?.Identity?.Name != null)
            {
                var user = users.GetUserByName(context.User.Identity.Name);
                if (user?.Policies.Contains("publishers.read") == true)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}