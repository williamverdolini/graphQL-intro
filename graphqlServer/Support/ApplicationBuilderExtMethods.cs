using System.Text;
using graphqlServer.Controllers.Auth;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Path = System.IO.Path;

namespace graphqlServer.Support
{
    public static class ApplicationBuilderExtMethods
    {
        public static IApplicationBuilder UseGraphQLSchemaPrint(this IApplicationBuilder builder, string artifactPath)
        {
            var resolver = builder.ApplicationServices.GetService<IRequestExecutorResolver>();
            if (resolver != null)
            {
                var executor = resolver.GetRequestExecutorAsync().Result;
                if (executor != null)
                {
                    var schemaFile = Path.Combine(artifactPath, "schema.graphql");
                    var newSchema = executor.Schema.ToString();
                    var printSchema = true;
                    if (File.Exists(schemaFile))
                    {
                        var oldSchema = File.ReadAllText(schemaFile);
                        printSchema = newSchema != oldSchema;
                    }
                    if (printSchema)
                    {
                        File.WriteAllText(schemaFile, newSchema);
                    }
                }
            }
            return builder;
        }

        public static IRequestExecutorBuilder SetSecurity(this IRequestExecutorBuilder builder)
        {
            var options = new HotChocolate.Execution.Options.RequestExecutorOptions
            {
                ExecutionTimeout = TimeSpan.FromSeconds(3),
                IncludeExceptionDetails = true,
            };
            options.Complexity.MaximumAllowed = 1000;
            options.Complexity.Enable = true;
            builder.SetRequestOptions(_ => options);
            return builder;
        }

        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            return services
                    .AddAuthorization(options =>
                        {
                            options.AddPolicy("publishers.read", policy =>
                                policy.Requirements.Add(new CanReadPublishersRequirement()));
                        })
                    .AddSingleton<IAuthorizationHandler, CanReadPublishersAuthorizationHandler>();
        }

        public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            return services
                    .AddTransient<IUserRepository, UserRepository>()
                    .AddTransient<ITokenService, TokenService>()
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                ValidIssuer = config["Jwt:Issuer"],
                                ValidAudience = config["Jwt:Issuer"],
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                            };
                    });
        }
    }
}