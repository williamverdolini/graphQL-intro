using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using GreenDonut;

namespace graphqlTests.Utils
{
    //see: https://github.com/ChilliCream/hotchocolate/blob/main/src/GreenDonut/test/Core.Tests/ManualBatchScheduler.cs
    public class ManualBatchScheduler : IBatchScheduler
    {
        private readonly object _sync = new object();
        private readonly ConcurrentQueue<Func<ValueTask>> _queue = new();

        public void Dispatch()
        {
            lock (_sync)
            {
                while (_queue.TryDequeue(out var dispatch))
                {
                    dispatch();
                }
            }
        }

        public void Schedule(Func<ValueTask> dispatch)
        {
            lock (_sync)
            {
                _queue.Enqueue(dispatch);
            }
        }
    }
}