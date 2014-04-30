using System;
using NUnit.Framework;
using StateMachines.Core.Utils;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class WorkflowExecutionContextTests
    {
        private Workflow m_workflow;
        private MakeMessageNode m_executionNode;
        private ConcatFunction m_concatFunction;
        private GetMessageFunction m_getMessage1Function;
        private GetMessageFunction m_getMessage2Function;
        private InitializeEventReceiver m_initEvent;

        [SetUp]
        public void Setup()
        {
            m_workflow = new Workflow();
            m_initEvent = new InitializeEventReceiver();
            m_executionNode = new MakeMessageNode();
            m_concatFunction = new ConcatFunction();
            m_getMessage1Function = new GetMessageFunction("Hello");
            m_getMessage2Function = new GetMessageFunction("World");

            m_workflow.Add(m_initEvent);
            m_workflow.Add(m_concatFunction);
            m_workflow.Add(m_getMessage1Function);
            m_workflow.Add(m_getMessage2Function);
            m_workflow.Add(m_executionNode);

            m_workflow.Connect(m_concatFunction.Pin(x => x.First), m_getMessage1Function.Pin(x => x.Message));
            m_workflow.Connect(m_concatFunction.Pin(x => x.Second), m_getMessage2Function.Pin(x => x.Message));
            m_workflow.Connect(m_executionNode.Pin(x => x.Input), m_concatFunction.Pin(x => x.Output));
            m_workflow.Connect(m_initEvent.Pin(x => x.Fired), m_executionNode);
        }

        [Test]
        public void Test_PublishEvent_ProvidesInputValues()
        {
            var executionContext = m_workflow.Compile();
            executionContext.PublishEvent(WorkflowEventData.Empty);

            Assert.That(m_executionNode.Output, Is.EqualTo("Hello World !!!"), "Message is not correct");
        }

        [Test]
        public void Test_Resume_ThrowsArgumentNull()
        {
            var executionContext = m_workflow.Compile();
            Assert.That(() => executionContext.Resume(null), Throws.TypeOf<ArgumentNullException>());
            
        }

        [Test]
        public void Test_Resume_ThrowsInvalidWorkflowStateException_IfNodeIsNotFound()
        {
            var executionContext = m_workflow.Compile();
            WorkflowStateData data = new WorkflowStateData();
            Assert.That(() => executionContext.Resume(data), Throws.TypeOf<InvalidWorkflowStateException>());
        }

        [Test]
        public void Test_Resume_ThrowsInvalidWorkflowStateException_IfNodeIsOfInvalidType()
        {
            var executionContext = m_workflow.Compile();
            WorkflowStateData data = new WorkflowStateData();
            data.ExecutingNodeGuid = m_concatFunction.Guid;
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