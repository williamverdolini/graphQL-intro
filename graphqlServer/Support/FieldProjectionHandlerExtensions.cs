using HotChocolate.Types;

namespace graphqlServer.Support
{
    public static class FieldProjectionHandlerExtensions
    {
        public static IObjectFieldDescriptor UseScalarProjection(this IObjectFieldDescriptor descriptor)
        {
            descriptor.Extend().OnBeforeCreate(x =>
            {
                x.ContextData["UseScalarProjectionKey"] = true;
            });
            return descriptor;
        }
    }
}