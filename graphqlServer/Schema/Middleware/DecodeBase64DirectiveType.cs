using System.Text;
using HotChocolate.Types;

namespace graphqlServer.Schema.Middleware
{
    public class DecodeBase64DirectiveType : DirectiveType
    {
        protected override void Configure(IDirectiveTypeDescriptor descriptor)
        {
            descriptor.Name("decodeBase64");
            descriptor.Location(DirectiveLocation.Field);
            descriptor.Use(next => async context =>
            {
                await next.Invoke(context);

                if (context.Result is string s)
                {
                    byte[] data = Convert.FromBase64String(s);
                    context.Result = Encoding.UTF8.GetString(data);
                }
            });
        }
    }
}