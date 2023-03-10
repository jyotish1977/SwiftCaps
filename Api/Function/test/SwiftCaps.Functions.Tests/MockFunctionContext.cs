using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace SwiftCaps.Functions.Tests
{
    class MockFunctionContext : FunctionContext, IDisposable
    {
        private readonly FunctionInvocation _invocation;
     
        public MockFunctionContext()
            : this(new MockFunctionDefinition(), new MockFunctionInvocation())
        {
        }

        public MockFunctionContext(FunctionDefinition functionDefinition, FunctionInvocation invocation)
        {
            FunctionDefinition = functionDefinition;
            _invocation = invocation;

            //Features.Set<IFunctionBindingsFeature>(new TestFunctionBindingsFeature
            //{
            //    OutputBindingsInfo = new DefaultOutputBindingsInfoProvider().GetBindingsInfo(FunctionDefinition)
            //});

            //BindingContext = new DefaultBindingContext(this);
        }

        public bool IsDisposed { get; private set; }

        public override IServiceProvider InstanceServices { get; set; }

        public override FunctionDefinition FunctionDefinition { get; }

        public override IDictionary<object, object> Items { get; set; }

        public override IInvocationFeatures Features { get; } /*= new InvocationFeatures(Enumerable.Empty<IInvocationFeatureProvider>());*/

        public override string InvocationId => _invocation.Id;

        public override string FunctionId => _invocation.FunctionId;

        public override TraceContext TraceContext => _invocation.TraceContext;

        public override BindingContext BindingContext { get; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
