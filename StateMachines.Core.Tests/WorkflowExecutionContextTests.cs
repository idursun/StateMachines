using System;
using NUnit.Framework;
using StateMachines.Core.Events;
using StateMachines.Core.Utils;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class WorkflowExecutionContextTests
    {
        private WorkflowBuilder m_workflowBuilder;
        private MakeMessageNode m_executionNode;
        private ConcatFunction m_concatFunction;
        private GetMessageFunction m_getMessage1Function;
        private GetMessageFunction m_getMessage2Function;
        private InitializeEventReceiver m_initEvent;

        [SetUp]
        public void Setup()
        {
            m_workflowBuilder = new WorkflowBuilder();
            m_initEvent = new InitializeEventReceiver();
            m_executionNode = new MakeMessageNode();
            m_concatFunction = new ConcatFunction();
            m_getMessage1Function = new GetMessageFunction("Hello");
            m_getMessage2Function = new GetMessageFunction("World");

            m_workflowBuilder.Add(m_initEvent);
            m_workflowBuilder.Add(m_concatFunction);
            m_workflowBuilder.Add(m_getMessage1Function);
            m_workflowBuilder.Add(m_getMessage2Function);
            m_workflowBuilder.Add(m_executionNode);

            m_workflowBuilder.Connect(m_concatFunction.Pin(x => x.First), m_getMessage1Function.Pin(x => x.Message));
            m_workflowBuilder.Connect(m_concatFunction.Pin(x => x.Second), m_getMessage2Function.Pin(x => x.Message));
            m_workflowBuilder.Connect(m_executionNode.Pin(x => x.Input), m_concatFunction.Pin(x => x.Output));
            m_workflowBuilder.Connect(m_initEvent.Pin(x => x.Fired), m_executionNode);
        }

        [Test]
        public void Test_PublishEvent_ProvidesInputValues()
        {
            var executionContext = m_workflowBuilder.Compile();
            executionContext.PublishEvent(WorkflowEventData.Empty);
            executionContext.Run();

            Assert.That(m_executionNode.Output, Is.EqualTo("Hello World !!!"), "Message is not correct");
        }

        [Test]
        public void Test_Resume_ThrowsArgumentNull()
        {
            var executionContext = m_workflowBuilder.Compile();
            Assert.That(() => executionContext.Resume(null), Throws.TypeOf<ArgumentNullException>());
            
        }

        [Test]
        public void Test_Resume_ThrowsInvalidWorkflowStateExceptionIfNodeIsNotFound()
        {
            var executionContext = m_workflowBuilder.Compile();
            WorkflowStateData data = new WorkflowStateData();
            Assert.That(() => executionContext.Resume(data), Throws.TypeOf<InvalidWorkflowStateException>());
        }

    }

    public class GetMessageFunction : WorkflowFunction
    {
        private readonly string m_initialMessage;

        public GetMessageFunction(string initialMessage)
        {
            m_initialMessage = initialMessage;
        }

        [Output]
        public string Message { get; set; }

        public override void Evaluate()
        {
            Message = m_initialMessage;
        }
    }
}