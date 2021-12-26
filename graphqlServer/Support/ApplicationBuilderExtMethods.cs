using HotChocolate.Execution;
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
    }
}