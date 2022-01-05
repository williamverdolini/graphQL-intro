#nullable enable

using System.Linq.Expressions;
using HotChocolate.Resolvers;
using HotChocolate.Types.Relay;

namespace HotChocolate.Types
{
    public static class NodeObjectTypeExtensions
    {
        public const string DbId = "dbId";

        /// <summary>
        /// Custom wrapper around built-in Relay node support.
        /// Additionally adds field for `dbId`.
        /// </summary>
        /// <remarks>
        /// see: https://gist.github.com/benmccallum/89d4d5b604d67094418956db43386ce5
        /// </remarks>
        /// <param name="descriptor"></param>
        /// <param name="idProperty">Expression to the property to use for the id.</param>
        /// <param name="nodeResolver">Resolver implementation.</param>
        /// <returns>The field descriptor for chaining.</returns>
        public static IObjectFieldDescriptor ImplementsNodeWithDbIdField<TNode, TId>(
            this IObjectTypeDescriptor<TNode> descriptor,
            Expression<Func<TNode, TId>> idProperty,
            NodeResolverDelegate<TNode, TId> nodeResolver)
            where TNode : class
        {
            // Add dbId which should just return the internal id as is
            var idPropertyFunc = idProperty.Compile();
            var dbIdFieldDescriptor = descriptor
                .DbIdField()
                .Resolve(ctx => idPropertyFunc(ctx.Parent<TNode>()));

            // This is a bit dodgy but not sure how else to force it.
            // Some seem to wanna be String not String!
            if (typeof(TId) == typeof(string))
            {
                dbIdFieldDescriptor.Type<NonNullType<StringType>>();
            }

            // Call the standard HC setup methods
            return descriptor
                .ImplementsNode()
                .IdField(idProperty)
                .ResolveNode(nodeResolver);
        }

        public static IObjectFieldDescriptor DbIdField<T>(this IObjectTypeDescriptor<T> descriptor)
            => descriptor.Field(DbId);

        public static IObjectFieldDescriptor DbIdField(this IObjectTypeDescriptor descriptor)
            => descriptor.Field(DbId);

        public static IInterfaceFieldDescriptor DbIdField<T>(this IInterfaceTypeDescriptor<T> descriptor)
            => descriptor.Field(DbId);

        public static IInterfaceFieldDescriptor DbIdField(this IInterfaceTypeDescriptor descriptor)
            => descriptor.Field(DbId);
    }
}