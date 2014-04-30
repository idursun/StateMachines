using Moq;
using NUnit.Framework;
using StateMachines.Core.Utils;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class WorkflowExecutionContextTests
    {
        private Workflow m_workflow;

        [SetUp]
        public void Setup()
        {
            m_workflow = new Workflow();
        }

        [Test]
        public void Test_PublishEvent_ProvidesInputValues()
        {
            var initEvent = new InitializeEventReceiver();
            var executionNode = new MakeMessageNode();
            var concatFunction = new ConcatFunction();
            var getMessage1Function = new GetMessageFunction("Hello");
            var getMessage2Function = new GetMessageFunction("World");

            m_workflow.Add(initEvent);
            m_workflow.Add(concatFunction);
            m_workflow.Add(getMessage1Function);
            m_workflow.Add(getMessage2Function);
            m_workflow.Add(executionNode);

            m_workflow.Connect(concatFunction.Pin(x => x.First), getMessage1Function.Pin(x => x.Message));
            m_workflow.Connect(concatFunction.Pin(x => x.Second), getMessage2Function.Pin(x => x.Message));
            m_workflow.Connect(executionNode.Pin(x => x.Input), concatFunction.Pin(x => x.Output));
            m_workflow.Connect(initEvent.Pin(x => x.Fired), executionNode);

            var executionContext = m_workflow.Compile();
            executionContext.PublishEvent(WorkflowEventData.Empty);

            Assert.That(executionNode.Output, Is.EqualTo("Hello World !!!"), "Message is not correct");
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