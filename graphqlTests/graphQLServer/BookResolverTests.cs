using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using NUnit.Framework;
using Snapshooter.NUnit;

namespace graphqlTests.graphQLServer
{
    [TestFixture]
    public class BookResolverTests : GraphQLServerBaseIntegrationTest
    {
        [Test]
        public async Task Books_verify_schema()
        {
            await BuildSchema()
                .MatchSnapshotAsync();
        }

        [Test]
        public async Task BookResolver_simple_string_request_test()
        {
            IExecutionResult result = await _requestBuilder.ExecuteRequestAsync(@"
            {
                books(first:1){
                    nodes {
                    title
                    }
                    totalCount
                }
            }");
            result.ToJson().MatchSnapshot();            
        }

        [Test]
        public async Task BookResolver_simple_query_test()
        {
            IReadOnlyQueryRequest request = FirstBookQuery();
            IExecutionResult result = await _requestBuilder.ExecuteRequestAsync(FirstBookQuery());

            result.ToJson().MatchSnapshot();
        }

        [Test]
        public async Task BookResolver_simple_query_with_specific_assertion()
        {
            IReadOnlyQueryRequest request = FirstBookQuery();
            IExecutionResult result = await _requestBuilder.ExecuteRequestAsync(FirstBookQuery());

            Assert.IsNotNull(result);
            Assert.IsNull(result.Errors);
            Assert.IsNull(result.ContextData);
            Assert.IsNull(result.Extensions);
            result.ToJson().Contains("Reconstituirea");
        }

        private static IReadOnlyQueryRequest FirstBookQuery()
        {
            return QueryRequestBuilder.New()
                    .SetQuery("{ books(first:1) { nodes { title } }}")
                    .Create();
        }
    }
}