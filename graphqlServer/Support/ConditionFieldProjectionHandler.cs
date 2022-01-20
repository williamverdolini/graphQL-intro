using HotChocolate.Data.MongoDb.Projections;
using HotChocolate.Execution.Processing;

namespace graphqlServer.Support
{
    public class ConditionFieldProjectionHandler : MongoDbProjectionScalarHandler
    {
        public override bool CanHandle(ISelection selection)
        {
            if (selection?.Field is not null)
            {
                if (selection.Field.ContextData.TryGetValue("UseScalarProjectionKey", out var val) && val != null)
                {
                    return (bool)val;
                }
            }

            return false;
        }
    }
}