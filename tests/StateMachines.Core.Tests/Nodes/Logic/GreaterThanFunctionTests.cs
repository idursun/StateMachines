using NUnit.Framework;
using StateMachines.Core.Events;
using StateMachines.Core.Nodes.Consts;
using StateMachines.Core.Nodes.Logic;
using StateMachines.Core.Utils;

namespace StateMachines.Core.Tests.Nodes.Logic
{
    [TestFixture]
    public class GreaterThanFunctionTests
    {
        [Test]
        public void Test_Evaluate_HandlesInteger()
        {
            GreaterThanFunction function = BuildAndRunWorkflow(10, 5); ;
            Assert.That(function.Result, Is.True);
        }

        [Test]
        public void Test_Evaluate_HandlesDouble()
        {
            GreaterThanFunction function = BuildAndRunWorkflow(10.0d, 5.0d); ;
            Assert.That(function.Result, Is.True);
        }

        [Test]
        public void Test_Evaluate_HandlesFloat()
        {
            GreaterThanFunction function = BuildAndRunWorkflow(10.0f, 5.0f); ;
            Assert.That(function.Result, Is.True);
        }

        [Test]
        public void Test_Evaluate_HandlesByte()
        {
            GreaterThanFunction function = BuildAndRunWorkflow<byte>(10, 1); ;
            Assert.That(function.Result, Is.True);
        }

        private static GreaterThanFunction BuildAndRunWorkflow<T>(T value1, T value2)
        {
            var intValue1 = new ConstValueFunction<T>(value1);
            var intValue2 = new ConstValueFunction<T>(value2);
            var function = new GreaterThanFunction();
            var dummyExecutionNode = new DummyExecutionNode();
            var initializeEventReceiver = new InitializeEventReceiver();

            WorkflowBuilder builder = new WorkflowBuilder();
            builder.Add(initializeEventReceiver);
            builder.Add(function);
            builder.Add(intValue1);
            builder.Add(intValue2);

            builder.Add(dummyExecutionNode);

            builder.Connect(function.Pin(x => x.A), intValue1.Pin(x => x.Value));
            builder.Connect(function.Pin(x => x.B), intValue2.Pin(x => x.Value));
            builder.Connect(function.Pin(x => x.Result), dummyExecutionNode.Pin(x => x.R));
            builder.Connect(initializeEventReceiver.Pin(x => x.Fired), dummyExecutionNode);

            IWorkflowExecutionContext context = builder.Compile();
            context.PublishEvent(new WorkflowEventData());
            context.Run();
            return function;
        }
    }


    public class DummyExecutionNode : WorkflowExecutionNode
    {
        [Input]
        public bool R { get; set; }

        public override void Execute(IWorkflowExecutionContext context)
        {
            
        }
    }
}