using System;
using Microsoft.Azure.Functions.Worker;

namespace SwiftCaps.Functions.Tests
{
    public class MockFunctionInvocation : FunctionInvocation
    {
        public MockFunctionInvocation(string id = null, string functionId = null)
        {
            if (id is not null)
            {
                Id = id;
            }

            if (functionId is not null)
            {
                FunctionId = functionId;
            }
        }

        public override string Id { get; } = Guid.NewGuid().ToString();

        public override string FunctionId { get; } = Guid.NewGuid().ToString();

        public override TraceContext TraceContext { get; } = new DefaultTraceContext(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
    }
}
