using NUnit.Framework;
using StateMachines.Core.Utils;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class EventReceiverTests
    {
        private Workflow m_workflow;

        [SetUp]
        public void Setup()
        {
            m_workflow = new Workflow();
        }

        [Test]
        public void Test_HandlesEventFromADerivedType()
        {
            var eventSink1 = new DataReceivedEventReceiver();
            var eventSink2 = new DataReceivedEventReceiver();
            var executionNode = new MakeMessageNode();
            var concatFunction = new ConcatFunction();

            m_workflow.Add(eventSink1);
            m_workflow.Add(eventSink2);
            m_workflow.Add(concatFunction);
            m_workflow.Add(executionNode);

            m_workflow.Connect(eventSink1.Pin(x => x.Data), concatFunction.Pin(x => x.First));
            m_workflow.Connect(eventSink2.Pin(x => x.Data), concatFunction.Pin(x => x.Second));
            m_workflow.Connect(concatFunction.Pin(x => x.Output), executionNode.Pin(x => x.Input));
            m_workflow.Connect(eventSink1.Pin(x => x.Next), executionNode);

            m_workflow.Compile();
            m_workflow.PublishEvent(new DataReceivedEventData()
            {
                Data = "DATA"
            });

            Assert.That(executionNode.Output, Is.EqualTo("DATA DATA !!!"), "Message was not correct");
        }
    }

    public class DataReceivedEventReceiver : WorkflowEventReceiver
    {
        [Output]
        public string Data { get; set; }

        public IExecutable Next { get; set; }

        public override void Execute(IWorkflowExecutionContext context)
        {
            context.Execute(Next);
        }

        public override void SetEventData(WorkflowEventData eventData)
        {
            DataReceivedEventData dataReceivedEventSink = eventData as DataReceivedEventData;
            if (dataReceivedEventSink != null)
            {
                Data = dataReceivedEventSink.Data;
            }
        }

        public override System.Type EventDataType
        {
            get { return typeof (DataReceivedEventData); }
        }
    }

    public class DataReceivedEventData : WorkflowEventData
    {
        public string Data { get; set; }
    }
}