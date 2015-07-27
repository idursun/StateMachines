﻿using NUnit.Framework;
using EventMachine.Core.Events;
using EventMachine.Core.Nodes.Consts;
using EventMachine.Core.Nodes.Logic;
using EventMachine.Core.Utils;

namespace EventMachine.Core.Tests.Nodes.Logic
{
    [TestFixture]
    public class GreaterThanFunctionTests
    {
        [Test]
        public void Test_Evaluate_HandlesInteger()
        {
            var function = BuildAndRunWorkflow(10, 5); ;
            Assert.That(function.Result, Is.True);
        }

        [Test]
        public void Test_Evaluate_HandlesDouble()
        {
            var function = BuildAndRunWorkflow(10.0d, 5.0d); ;
            Assert.That(function.Result, Is.True);
        }

        [Test]
        public void Test_Evaluate_HandlesFloat()
        {
            var function = BuildAndRunWorkflow(10.0f, 5.0f); ;
            Assert.That(function.Result, Is.True);
        }

        [Test]
        public void Test_Evaluate_HandlesByte()
        {
            var function = BuildAndRunWorkflow<byte>(10, 1); ;
            Assert.That(function.Result, Is.True);
        }

        private static GreaterThanFunction BuildAndRunWorkflow<T>(T value1, T value2)
        {
            var intValue1 = new ConstValueFunction<T>(value1);
            var intValue2 = new ConstValueFunction<T>(value2);
            var function = new GreaterThanFunction();
            var dummyExecutionNode = new DummyExecutionNode();
            var initializeEventReceiver = new InitializeEventReceiver();

            var builder = new WorkflowBuilder()
                .Connect(_ => intValue1.Value, _ => function.A)
                .Connect(_ => intValue2.Value, _ => function.B)
                .Connect(_ => function.Result, _ => dummyExecutionNode.R)
                .Connect(_ => initializeEventReceiver.Fired, dummyExecutionNode);

            var context = builder.Compile();
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